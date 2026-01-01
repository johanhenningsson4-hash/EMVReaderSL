/*=========================================================================================
'  Copyright(C):    Johan Henningsson 
'  
'  Author :         Johan Henningsson
'
'  Module :         EMVReader.cs
'   
'  Date   :         June 23, 2008
'==========================================================================================*/

using NfcReaderLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EMVCard
{
    public partial class MainEMVReaderBin : Form
    {
        // Business logic classes
        private EmvCardReader _cardReader;
        private EmvDataParser _dataParser;
        private EmvRecordReader _recordReader;
        private EmvApplicationSelector _appSelector;
        private EmvGpoProcessor _gpoProcessor;
        private EmvTokenGenerator _tokenGenerator;

        // Current card data
        private EmvDataParser.EmvCardData _currentCardData;
        
        // Application list
        private List<EmvApplicationSelector.ApplicationInfo> _applications;
        private Dictionary<string, EmvApplicationSelector.ApplicationInfo> _appDisplayNameToInfo;

        // Configuration
        private bool _maskPAN = false;
        private bool _isPolling = false;
        private int _pollCount = 0;
        private int _maxPolls = 0;
        private System.Windows.Forms.Timer _pollTimer;

        public MainEMVReaderBin() {
            InitializeComponent();
            InitializeEmvComponents();
            InitializePolling();
        }

        /// <summary>
        /// Initialize EMV business logic components.
        /// </summary>
        private void InitializeEmvComponents()
        {
            _cardReader = new EmvCardReader();
            _dataParser = new EmvDataParser();
            _currentCardData = new EmvDataParser.EmvCardData();
            _appDisplayNameToInfo = new Dictionary<string, EmvApplicationSelector.ApplicationInfo>();
            
            // Wire up logging events
            _cardReader.LogMessage += (s, msg) => displayOut(0, 0, msg);
            _dataParser.LogMessage += (s, msg) => displayOut(0, 0, msg);
        }

        /// <summary>
        /// Initialize polling timer.
        /// </summary>
        private void InitializePolling()
        {
            _pollTimer = new System.Windows.Forms.Timer();
            _pollTimer.Interval = 2000; // 2 seconds between polls
            _pollTimer.Tick += PollTimer_Tick;
        }

        /// <summary>
        /// Helper method to safely update UI from any thread.
        /// </summary>
        private void SafeUpdateUI(Action action)
        {
            if (InvokeRequired)
            {
                Invoke(action);
            }
            else
            {
                action();
            }
        }

        private void ClearBuffers() {
            _currentCardData.Clear();
            textCardNum.Text = "";
            textEXP.Text = "";
            textHolder.Text = "";
            textTrack.Text = "";
            textIccCert.Text = "";
            txtSLToken.Text = "";
            cbPSE.Items.Clear();
            cbPSE.Text = "";
            _appDisplayNameToInfo?.Clear();
        }

        private void displayOut(int errType, int retVal, string PrintText) {
            // Check if we need to invoke on UI thread
            if (richTextBoxLogs.InvokeRequired)
            {
                richTextBoxLogs.Invoke(new Action(() => displayOut(errType, retVal, PrintText)));
                return;
            }

            switch (errType) {
                case 0:
                    break;
                case 1:
                    PrintText = ModWinsCard64.GetScardErrMsg(retVal);
                    break;
                case 2:
                    PrintText = "<" + PrintText;
                    break;
                case 3:
                    PrintText = "> " + PrintText;
                    break;
            }

            richTextBoxLogs.Select(richTextBoxLogs.Text.Length, 0);
            richTextBoxLogs.SelectedText = PrintText + "\r\n";
            richTextBoxLogs.ScrollToCaret();
        }

        private void EnableButtons() {
            bInit.Enabled = false;
            bConnect.Enabled = true;
            bReset.Enabled = true;
            bClear.Enabled = true;
        }

        private async void bInit_Click(object sender, EventArgs e) {
            var readers = await _cardReader.InitializeAsync();
            
            if (readers.Count == 0)
            {
                displayOut(0, 0, "No card readers found");
                return;
            }

            EnableButtons();

            // Populate reader dropdown
            cbReader.Items.Clear();
            foreach (var reader in readers)
            {
                cbReader.Items.Add(reader);
            }

            if (cbReader.Items.Count > 0)
                cbReader.SelectedIndex = 0;
        }

        private async void bConnect_Click(object sender, EventArgs e) {
            ClearBuffers();

            if (await _cardReader.ConnectAsync(cbReader.Text))
            {
                displayOut(0, 0, $"Successfully connected to {cbReader.Text}");
            }
        }

        private async void bReadApp_Click(object sender, EventArgs e) {
            // Check if an application is selected BEFORE clearing buffers
            if (string.IsNullOrEmpty(cbPSE.Text) || !_appDisplayNameToInfo.ContainsKey(cbPSE.Text))
            {
                displayOut(0, 0, "Please select an application");
                return;
            }

            var selectedApp = _appDisplayNameToInfo[cbPSE.Text];
            
            // Now clear only the card data fields (not the application selection)
            _currentCardData.Clear();
            textCardNum.Text = "";
            textEXP.Text = "";
            textHolder.Text = "";
            textTrack.Text = "";
            textIccCert.Text = "";
            txtSLToken.Text = "";
            
            // Select application
            var (selectSuccess, fciData) = await _appSelector.SelectApplicationAsync(selectedApp.AID);
            if (!selectSuccess)
            {
                displayOut(0, 0, "Select AID Failed");
                return;
            }

            // Initialize components if needed
            if (_gpoProcessor == null)
            {
                _gpoProcessor = new EmvGpoProcessor(_cardReader);
                _gpoProcessor.LogMessage += (s, msg) => displayOut(0, 0, msg);
            }

            if (_recordReader == null)
            {
                _recordReader = new EmvRecordReader(_cardReader, _dataParser);
                _recordReader.LogMessage += (s, msg) => displayOut(0, 0, msg);
            }

            if (_tokenGenerator == null)
            {
                _tokenGenerator = new EmvTokenGenerator();
                _tokenGenerator.LogMessage += (s, msg) => displayOut(0, 0, msg);
            }

            // Send GPO
            var (gpoSuccess, gpoResponse) = await _gpoProcessor.SendGPOAsync(fciData);
            
            if (!gpoSuccess)
            {
                displayOut(0, 0, "Send GPO Failed - attempting to read common records anyway");
            }

            if (gpoSuccess)
            {
                // Parse GPO response
                _dataParser.ParseTLV(gpoResponse, 0, gpoResponse.Length - 2, _currentCardData, 0);

                // Parse AFL and read records
                var aflList = _dataParser.ParseAFL(gpoResponse, gpoResponse.Length);
                
                if (aflList.Count > 0)
                {
                    await _recordReader.ReadAFLRecordsAsync(aflList, _currentCardData);
                }
                else
                {
                    displayOut(0, 0, "Could not parse AFL, attempting to read common records");
                    await _recordReader.TryReadCommonRecordsAsync(_currentCardData);
                }
            }
            else
            {
                // Try reading common records
                displayOut(0, 0, "Since GPO failed, attempting to read common records directly");
                await _recordReader.TryReadCommonRecordsAsync(_currentCardData);
            }

            // Extract from Track2 if needed
            _dataParser.ExtractFromTrack2(_currentCardData);

            // Update UI
            UpdateUIFromCardData();

            // Generate SL Token
            var tokenResult = await _tokenGenerator.GenerateTokenAsync(_currentCardData, _currentCardData.PAN, selectedApp.AID);
            
            // Update UI on UI thread
            SafeUpdateUI(() =>
            {
                if (tokenResult.Success)
                {
                    txtSLToken.Text = tokenResult.Token;
                }
                else
                {
                    txtSLToken.Text = $"Error: {tokenResult.ErrorMessage}";
                }
            });
        }

        /// <summary>
        /// Update UI fields from card data.
        /// </summary>
        private void UpdateUIFromCardData()
        {
            SafeUpdateUI(() =>
            {
                // Apply PAN masking if enabled
                if (_maskPAN && !string.IsNullOrEmpty(_currentCardData.PAN))
                {
                    textCardNum.Text = Util.MaskPAN(_currentCardData.PAN);
                }
                else
                {
                    textCardNum.Text = _currentCardData.PAN ?? "";
                }
                
                textEXP.Text = _currentCardData.ExpiryDate ?? "";
                textHolder.Text = _currentCardData.CardholderName ?? "";
                textTrack.Text = _currentCardData.Track2Data ?? "";
                textIccCert.Text = _currentCardData.IccCertificate ?? "";
            });
        }
        private async void bLoadPSE_Click(object sender, EventArgs e) {
            ClearBuffers();
            
            if (_appSelector == null)
            {
                _appSelector = new EmvApplicationSelector(_cardReader);
                _appSelector.LogMessage += (s, msg) => displayOut(0, 0, msg);
            }

            _applications = await _appSelector.LoadPSEAsync();
            
            // Populate dropdown and dictionary
            cbPSE.Items.Clear();
            _appDisplayNameToInfo.Clear();
            
            for (int i = 0; i < _applications.Count; i++)
            {
                var app = _applications[i];
                string itemName = $"{i + 1}. {app.DisplayName}";
                cbPSE.Items.Add(itemName);
                _appDisplayNameToInfo[itemName] = app;
            }

            if (cbPSE.Items.Count > 0)
            {
                cbPSE.SelectedIndex = 0;
            }
        }

        private async void bLoadPPSE_Click(object sender, EventArgs e) {
            ClearBuffers();
            
            if (_appSelector == null)
            {
                _appSelector = new EmvApplicationSelector(_cardReader);
                _appSelector.LogMessage += (s, msg) => displayOut(0, 0, msg);
            }

            _applications = await _appSelector.LoadPPSEAsync();
            
            // Populate dropdown and dictionary
            cbPSE.Items.Clear();
            _appDisplayNameToInfo.Clear();
            
            for (int i = 0; i < _applications.Count; i++)
            {
                var app = _applications[i];
                string itemName = $"{i + 1}. {app.DisplayName}";
                cbPSE.Items.Add(itemName);
                _appDisplayNameToInfo[itemName] = app;
            }

            if (cbPSE.Items.Count > 0)
            {
                cbPSE.SelectedIndex = 0;
            }
        }

        private void bClear_Click(object sender, EventArgs e) {
            richTextBoxLogs.Clear();
        }

        private async void bReset_Click(object sender, EventArgs e) {
            await _cardReader?.DisconnectAsync();
            await _cardReader?.ReleaseAsync();
            
            // Reinitialize
            InitializeEmvComponents();
            
            cbReader.Items.Clear();
            cbReader.Text = "";
            bInit.Enabled = true;
            ClearBuffers();
            richTextBoxLogs.Clear();
        }

        private async void bQuit_Click(object sender, EventArgs e) {
            await _cardReader?.DisconnectAsync();
            await _cardReader?.ReleaseAsync();
            System.Environment.Exit(0);
        }

        /// <summary>
        /// Handle PAN masking checkbox change event.
        /// </summary>
        private void chkMaskPAN_CheckedChanged(object sender, EventArgs e)
        {
            _maskPAN = chkMaskPAN.Checked;
            
            // Re-display the PAN with new masking setting if card data exists
            if (!string.IsNullOrEmpty(_currentCardData.PAN))
            {
                if (_maskPAN)
                {
                    textCardNum.Text = Util.MaskPAN(_currentCardData.PAN);
                    displayOut(0, 0, "PAN masking enabled");
                }
                else
                {
                    textCardNum.Text = _currentCardData.PAN;
                    displayOut(0, 0, "PAN masking disabled - showing full card number");
                }
            }
        }

        /// <summary>
        /// Start polling for card reads.
        /// </summary>
        private async void btnStartPoll_Click(object sender, EventArgs e)
        {
            if (_isPolling)
            {
                displayOut(0, 0, "Polling already in progress");
                return;
            }

            // Check if application is selected
            if (string.IsNullOrEmpty(cbPSE.Text) || !_appDisplayNameToInfo.ContainsKey(cbPSE.Text))
            {
                displayOut(0, 0, "Please select an application before starting poll");
                return;
            }

            // Get poll count from numeric updown
            _maxPolls = (int)numPollCount.Value;
            if (_maxPolls <= 0)
            {
                displayOut(0, 0, "Please enter a valid poll count (greater than 0)");
                return;
            }

            _pollCount = 0;
            _isPolling = true;
            
            // Update UI
            btnStartPoll.Enabled = false;
            btnStopPoll.Enabled = true;
            numPollCount.Enabled = false;
            
            displayOut(0, 0, $"Starting polling: {_maxPolls} reads, interval: {_pollTimer.Interval}ms");
            
            // Start first read immediately
            await PerformCardReadAsync();
            
            // Start timer for subsequent reads
            _pollTimer.Start();
        }

        /// <summary>
        /// Stop polling for card reads.
        /// </summary>
        private void btnStopPoll_Click(object sender, EventArgs e)
        {
            StopPolling();
            displayOut(0, 0, $"Polling stopped by user. Completed {_pollCount} of {_maxPolls} reads");
        }

        /// <summary>
        /// Timer tick event for polling.
        /// </summary>
        private async void PollTimer_Tick(object sender, EventArgs e)
        {
            if (!_isPolling)
            {
                _pollTimer.Stop();
                return;
            }

            if (_pollCount >= _maxPolls)
            {
                StopPolling();
                displayOut(0, 0, $"Polling completed: {_pollCount} reads finished");
                return;
            }

            await PerformCardReadAsync();
        }

        /// <summary>
        /// Perform a single card read operation (synchronous wrapper for backward compatibility).
        /// </summary>
        private void PerformCardRead()
        {
            // Call async version and wait
            Task.Run(async () => await PerformCardReadAsync()).Wait();
        }

        /// <summary>
        /// Perform a single card read operation asynchronously.
        /// </summary>
        private async Task PerformCardReadAsync()
        {
            _pollCount++;
            displayOut(0, 0, $"--- Poll {_pollCount} of {_maxPolls} ---");
            
            try
            {
                // Check/reconnect to card before each poll
                if (!await EnsureCardConnectionAsync())
                {
                    displayOut(0, 0, $"Poll {_pollCount}: Waiting for card...");
                    return;
                }

                // Use the existing bReadApp_Click logic
                var selectedApp = _appDisplayNameToInfo[cbPSE.Text];
                
                // Clear only card data fields
                _currentCardData.Clear();
                
                // Select application
                var (selectSuccess, fciData) = await _appSelector.SelectApplicationAsync(selectedApp.AID);
                if (!selectSuccess)
                {
                    displayOut(0, 0, $"Poll {_pollCount}: Select AID Failed");
                    return;
                }

                // Initialize components if needed
                if (_gpoProcessor == null)
                {
                    _gpoProcessor = new EmvGpoProcessor(_cardReader);
                    _gpoProcessor.LogMessage += (s, msg) => displayOut(0, 0, msg);
                }

                if (_recordReader == null)
                {
                    _recordReader = new EmvRecordReader(_cardReader, _dataParser);
                    _recordReader.LogMessage += (s, msg) => displayOut(0, 0, msg);
                }

                if (_tokenGenerator == null)
                {
                    _tokenGenerator = new EmvTokenGenerator();
                    _tokenGenerator.LogMessage += (s, msg) => displayOut(0, 0, msg);
                }

                // Send GPO
                var (gpoSuccess, gpoResponse) = await _gpoProcessor.SendGPOAsync(fciData);
                
                if (gpoSuccess)
                {
                    _dataParser.ParseTLV(gpoResponse, 0, gpoResponse.Length - 2, _currentCardData, 0);
                    var aflList = _dataParser.ParseAFL(gpoResponse, gpoResponse.Length);
                    
                    if (aflList.Count > 0)
                    {
                        await _recordReader.ReadAFLRecordsAsync(aflList, _currentCardData);
                    }
                    else
                    {
                        await _recordReader.TryReadCommonRecordsAsync(_currentCardData);
                    }
                }
                else
                {
                    await _recordReader.TryReadCommonRecordsAsync(_currentCardData);
                }

                _dataParser.ExtractFromTrack2(_currentCardData);
                UpdateUIFromCardData();

                // Generate SL Token
                var tokenResult = await _tokenGenerator.GenerateTokenAsync(_currentCardData, _currentCardData.PAN, selectedApp.AID);
                
                // Update UI on UI thread
                SafeUpdateUI(() =>
                {
                    if (tokenResult.Success)
                    {
                        txtSLToken.Text = tokenResult.Token;
                        displayOut(0, 0, $"Poll {_pollCount}: Success - PAN: {(_maskPAN ? Util.MaskPAN(_currentCardData.PAN) : _currentCardData.PAN)}");
                    }
                    else
                    {
                        txtSLToken.Text = $"Error: {tokenResult.ErrorMessage}";
                        displayOut(0, 0, $"Poll {_pollCount}: Token generation failed");
                    }
                });
            }
            catch (Exception ex)
            {
                displayOut(0, 0, $"Poll {_pollCount}: Error - {ex.Message}");
            }
        }

        /// <summary>
        /// Ensure card is connected before reading. Attempts reconnection if needed.
        /// </summary>
        /// <returns>True if card is ready, false if waiting for card</returns>
        private bool EnsureCardConnection()
        {
            try
            {
                // Always try to reconnect to detect card changes
                // This will fail quickly if no card is present
                if (!string.IsNullOrEmpty(cbReader.Text))
                {
                    // Disconnect first to reset state
                    if (_cardReader.IsConnected)
                    {
                        _cardReader.Disconnect();
                    }

                    // Try to connect
                    bool connected = _cardReader.Connect(cbReader.Text);
                    if (connected)
                    {
                        displayOut(0, 0, "Card detected and connected");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                displayOut(0, 0, $"Card connection check failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Ensure card is connected before reading asynchronously. Attempts reconnection if needed.
        /// </summary>
        /// <returns>True if card is ready, false if waiting for card</returns>
        private async Task<bool> EnsureCardConnectionAsync()
        {
            try
            {
                // Always try to reconnect to detect card changes
                // This will fail quickly if no card is present
                if (!string.IsNullOrEmpty(cbReader.Text))
                {
                    // Disconnect first to reset state
                    if (_cardReader.IsConnected)
                    {
                        await _cardReader.DisconnectAsync();
                    }

                    // Try to connect
                    bool connected = await _cardReader.ConnectAsync(cbReader.Text);
                    if (connected)
                    {
                        displayOut(0, 0, "Card detected and connected");
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                displayOut(0, 0, $"Card connection check failed: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Stop polling and reset UI.
        /// </summary>
        private void StopPolling()
        {
            _isPolling = false;
            _pollTimer.Stop();
            
            // Update UI
            btnStartPoll.Enabled = true;
            btnStopPoll.Enabled = false;
            numPollCount.Enabled = true;
        }
    }
}