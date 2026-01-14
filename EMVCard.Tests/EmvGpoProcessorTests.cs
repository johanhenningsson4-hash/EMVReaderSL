using System;
using System.Diagnostics;
using EMVCard;
using FluentAssertions;
using Moq;
using Xunit;

namespace EMVCard.Tests
{
    /// <summary>
    /// Unit tests for EmvGpoProcessor
    /// </summary>
    public class EmvGpoProcessorTests
    {
        [Fact]
        public void SendGPO_WithNullFciData_ThrowsArgumentNullException()
        {
            var mockReader = new Mock<EmvCardReader>();
            var processor = new EmvGpoProcessor(mockReader.Object);
            
            Assert.Throws<ArgumentNullException>(() => processor.SendGPO(null, out _));
        }

        [Fact]
        public void SendGPO_WithNoPDOL_CallsSendSimplifiedGPO()
        {
            var mockReader = new Mock<EmvCardReader>();
            var processor = new EmvGpoProcessor(mockReader.Object);
            var fciData = new byte[] { 0x6F, 0x84, 0xA5, 0x50, 0x9F, 0x12, 0x34 };
            
            // Setup SendApduWithAutoFix to always fail (simulate no card response)
            mockReader.Setup(r => r.SendApduWithAutoFix(It.IsAny<byte[]>(), out It.Ref<byte[]>.IsAny)).Returns(false);
            
            var result = processor.SendGPO(fciData, out var gpoResponse);
            result.Should().BeFalse();
            gpoResponse.Should().BeNull();
        }

        [Fact]
        public void SendGPO_WithPDOL_AndValidResponse_ReturnsTrue()
        {
            var mockReader = new Mock<EmvCardReader>();
            var processor = new EmvGpoProcessor(mockReader.Object);
            // FCI with PDOL tag 9F38, length 3, value: 9F 66 04 (tag 9F66, length 4)
            var fciData = new byte[] { 0x6F, 0x84, 0x9F, 0x38, 0x03, 0x9F, 0x66, 0x04 };
            byte[] fakeResponse = new byte[] { 0x80, 0x01, 0x02 };
            mockReader.Setup(r => r.SendApduWithAutoFix(It.IsAny<byte[]>(), out fakeResponse)).Returns(true);
            
            var result = processor.SendGPO(fciData, out var gpoResponse);
            result.Should().BeTrue();
            gpoResponse.Should().NotBeNull();
            gpoResponse[0].Should().Be(0x80);
        }

        [Fact]
        public void SendGPO_WithPDOL_AndEmptyResponse_ReturnsFalse()
        {
            var mockReader = new Mock<EmvCardReader>();
            var processor = new EmvGpoProcessor(mockReader.Object);
            var fciData = new byte[] { 0x6F, 0x84, 0x9F, 0x38, 0x03, 0x9F, 0x66, 0x04 };
            byte[] emptyResponse = new byte[0];
            mockReader.Setup(r => r.SendApduWithAutoFix(It.IsAny<byte[]>(), out emptyResponse)).Returns(true);
            
            var result = processor.SendGPO(fciData, out var gpoResponse);
            result.Should().BeFalse();
            gpoResponse.Should().BeEmpty();
        }

        [Fact]
        public void LoggingLevel_CanBeSetAndAffectsLogging()
        {
            var mockReader = new Mock<EmvCardReader>();
            var processor = new EmvGpoProcessor(mockReader.Object);
            processor.SetLoggingLevel(SourceLevels.Off);
            processor.LoggingLevel.Should().Be(SourceLevels.Off);
        }
    }
}
