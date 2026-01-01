using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMVCard
{
    /// <summary>
    /// Handles PSE/PPSE application selection and enumeration.
    /// </summary>
    public class EmvApplicationSelector
    {
        private static readonly TraceSource _traceSource = new TraceSource("EMVCard.EmvApplicationSelector");

        private readonly EmvCardReader _cardReader;

        public event EventHandler<string> LogMessage;

        public EmvApplicationSelector(EmvCardReader cardReader)
        {
            _cardReader = cardReader ?? throw new ArgumentNullException(nameof(cardReader));
        }

        /// <summary>
        /// Application information.
        /// </summary>
        public class ApplicationInfo
        {
            public string AID { get; set; }
            public string Label { get; set; }
            public string PreferredName { get; set; }
            public byte Priority { get; set; }

            public string DisplayName => !string.IsNullOrEmpty(Label) ? Label : $"App_{AID?.Substring(Math.Max(0, (AID?.Length ?? 0) - 4))}";
        }

        /// <summary>
        /// Load applications using PSE (1PAY.SYS.DDF01) - for contact cards.
        /// </summary>
        /// <returns>List of available applications</returns>
        public List<ApplicationInfo> LoadPSE()
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "LoadPSE: Loading contact PSE");

            var applications = new List<ApplicationInfo>();

            // Select PSE application
            byte[] selectPSE = new byte[]
            {
                0x00, 0xA4, 0x04, 0x00, 0x0E,
                0x31, 0x50, 0x41, 0x59, 0x2E, 0x53, 0x59, 0x53, 0x2E, 0x44, 0x44, 0x46, 0x30, 0x31
            };

            if (!_cardReader.SendApduWithAutoFix(selectPSE, out byte[] response))
            {
                OnLogMessage("Select PSE Application Failed");
                return applications;
            }

            if (response.Length < 2 || response[response.Length - 2] != 0x90 || response[response.Length - 1] != 0x00)
            {
                OnLogMessage("Select PSE Application Failed - invalid response");
                return applications;
            }

            // Read records from SFI 1
            for (int record = 1; record < 32; record++)
            {
                byte[] readRecord = new byte[] { 0x00, 0xB2, (byte)record, 0x0C, 0x00 };

                if (!_cardReader.SendApduWithAutoFix(readRecord, out response))
                {
                    break;
                }

                // Check for "Record not found" (6A 83)
                if (response.Length == 2 && response[0] == 0x6A && response[1] == 0x83)
                {
                    OnLogMessage($"Record {record} does not exist, ending AID reading");
                    break;
                }

                // Check success
                if (response.Length < 2 || response[response.Length - 2] != 0x90 || response[response.Length - 1] != 0x00)
                {
                    OnLogMessage($"Record {record} read failed, stopping");
                    break;
                }

                OnLogMessage($"Parsing AID information in Record {record}");
                ParseApplicationRecord(response, response.Length - 2, applications);
            }

            _traceSource.TraceEvent(TraceEventType.Information, 0, $"LoadPSE: Found {applications.Count} applications");
            return applications;
        }

        /// <summary>
        /// Load applications using PPSE (2PAY.SYS.DDF01) - for contactless cards.
        /// </summary>
        /// <returns>List of available applications</returns>
        public List<ApplicationInfo> LoadPPSE()
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, "LoadPPSE: Loading contactless PPSE");

            var applications = new List<ApplicationInfo>();

            // Select PPSE application
            byte[] selectPPSE = new byte[]
            {
                0x00, 0xA4, 0x04, 0x00, 0x0E,
                0x32, 0x50, 0x41, 0x59, 0x2E, 0x53, 0x59, 0x53, 0x2E, 0x44, 0x44, 0x46, 0x30, 0x31
            };

            if (!_cardReader.SendApduWithAutoFix(selectPPSE, out byte[] response))
            {
                OnLogMessage("Select PPSE Application Failed");
                return applications;
            }

            if (response.Length < 2 || response[response.Length - 2] != 0x90 || response[response.Length - 1] != 0x00)
            {
                OnLogMessage("Select PPSE Application Failed - invalid response");
                return applications;
            }

            // Parse FCI Template for Application Templates (61)
            int index = 0;
            while (index < response.Length - 2)
            {
                if (response[index] == 0x61)
                {
                    int len = response[index + 1];
                    int start = index + 2;
                    int end = start + len;

                    if (end > response.Length - 2)
                        break;

                    var app = ParseApplicationTemplate(response, start, end);
                    if (app != null && !string.IsNullOrEmpty(app.AID))
                    {
                        applications.Add(app);
                    }

                    index = end;
                }
                else
                {
                    index++;
                }
            }

            _traceSource.TraceEvent(TraceEventType.Information, 0, $"LoadPPSE: Found {applications.Count} applications");
            return applications;
        }

        /// <summary>
        /// Load applications using PSE (1PAY.SYS.DDF01) asynchronously - for contact cards.
        /// </summary>
        /// <returns>List of available applications</returns>
        public Task<List<ApplicationInfo>> LoadPSEAsync()
        {
            return Task.Run(() => LoadPSE());
        }

        /// <summary>
        /// Load applications using PPSE (2PAY.SYS.DDF01) asynchronously - for contactless cards.
        /// </summary>
        /// <returns>List of available applications</returns>
        public Task<List<ApplicationInfo>> LoadPPSEAsync()
        {
            return Task.Run(() => LoadPPSE());
        }

        /// <summary>
        /// Parse application record from PSE response.
        /// </summary>
        private void ParseApplicationRecord(byte[] buffer, int length, List<ApplicationInfo> applications)
        {
            string currentAID = "";
            int index = 0;

            while (index < length)
            {
                byte tag = buffer[index++];
                byte? tag2 = null;

                // Two-byte tag
                if ((tag & 0x1F) == 0x1F)
                {
                    if (index >= length)
                        break;
                    tag2 = buffer[index++];
                }

                int tagValue = tag2.HasValue ? (tag << 8 | tag2.Value) : tag;

                // Get length
                if (index >= length)
                    break;

                int len = buffer[index++];
                if (len > 0x80)
                {
                    int lenLen = len & 0x7F;
                    len = 0;
                    for (int i = 0; i < lenLen; i++)
                    {
                        if (index >= length)
                            break;
                        len = (len << 8) + buffer[index++];
                    }
                }

                // Get value
                if (index + len > length)
                    break;

                byte[] value = new byte[len];
                Array.Copy(buffer, index, value, 0, len);
                index += len;

                // Process tags
                switch (tagValue)
                {
                    case 0x4F: // AID
                        currentAID = string.Join(" ", value.Select(b => b.ToString("X2")));
                        OnLogMessage($"AID: {currentAID}");
                        break;

                    case 0x50: // Application Label
                        string label = Encoding.ASCII.GetString(value).Trim();
                        OnLogMessage($"Application Label: {label}");
                        
                        var app = new ApplicationInfo { AID = currentAID, Label = label };
                        if (!applications.Any(a => a.AID == currentAID))
                        {
                            applications.Add(app);
                        }
                        break;

                    case 0x9F12: // Preferred Name
                        OnLogMessage($"Preferred Name: {Encoding.ASCII.GetString(value)}");
                        break;

                    case 0x87: // Priority
                        OnLogMessage($"Application Priority: {value[0]}");
                        break;

                    case 0x61: // Application Template
                    case 0x70: // FCI template
                        OnLogMessage($"Template tag {tagValue:X}, parsing inner TLVs...");
                        ParseApplicationRecord(value, len, applications);
                        break;
                }
            }
        }

        /// <summary>
        /// Parse application template from PPSE response.
        /// </summary>
        private ApplicationInfo ParseApplicationTemplate(byte[] buffer, int start, int end)
        {
            var app = new ApplicationInfo();
            int index = start;

            while (index < end)
            {
                if (index >= buffer.Length)
                    break;

                byte tag = buffer[index++];
                if (index >= end)
                    break;

                int tagLen = buffer[index++];
                if (index + tagLen > end)
                    break;

                byte[] value = new byte[tagLen];
                Array.Copy(buffer, index, value, 0, tagLen);
                index += tagLen;

                switch (tag)
                {
                    case 0x4F: // AID
                        app.AID = string.Join(" ", value.Select(b => b.ToString("X2")));
                        OnLogMessage($"AID: {app.AID}");
                        break;

                    case 0x50: // Application Label
                        app.Label = Encoding.ASCII.GetString(value).Trim();
                        OnLogMessage($"Application Label: {app.Label}");
                        break;
                }
            }

            return app;
        }

        /// <summary>
        /// Select specific application by AID.
        /// </summary>
        /// <param name="aid">Application AID (space-separated hex)</param>
        /// <param name="fciData">FCI data returned from selection</param>
        /// <returns>True if selection successful</returns>
        public bool SelectApplication(string aid, out byte[] fciData)
        {
            _traceSource.TraceEvent(TraceEventType.Information, 0, $"SelectApplication: Selecting AID {aid}");

            fciData = null;

            // Build SELECT command
            string[] aidBytes = aid.Split(' ');
            List<byte> selectCmd = new List<byte> { 0x00, 0xA4, 0x04, 0x00, (byte)aidBytes.Length };
            
            foreach (string b in aidBytes)
            {
                selectCmd.Add(Convert.ToByte(b, 16));
            }

            if (!_cardReader.SendApduWithAutoFix(selectCmd.ToArray(), out byte[] response))
            {
                OnLogMessage("Select AID Failed - transmission error");
                return false;
            }

            if (response.Length < 2 || response[response.Length - 2] != 0x90 || response[response.Length - 1] != 0x00)
            {
                OnLogMessage("Select AID Failed - invalid response");
                return false;
            }

            fciData = response;
            OnLogMessage($"Successfully selected application: {aid}");
            return true;
        }

        /// <summary>
        /// Select specific application by AID asynchronously.
        /// </summary>
        /// <param name="aid">Application AID (space-separated hex)</param>
        /// <returns>Tuple containing success flag and FCI data</returns>
        public Task<(bool success, byte[] fciData)> SelectApplicationAsync(string aid)
        {
            return Task.Run(() =>
            {
                bool success = SelectApplication(aid, out byte[] fciData);
                return (success, fciData);
            });
        }

        protected virtual void OnLogMessage(string message)
        {
            LogMessage?.Invoke(this, message);
        }
    }
}
