using System;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Moq;
using KRPC.Server;
using KRPC.Server.RPC;

namespace KRPCTest.Server.RPC
{
    [TestFixture]
    public class RPCServerTest
    {
        byte[] helloMessage;

        [SetUp]
        public void SetUp ()
        {
            helloMessage = new byte[8 + 32];
            byte[] header = { 0x48, 0x45, 0x4C, 0x4C, 0x4F, 0xBA, 0xDA, 0x55 };
            Array.Copy (header, helloMessage, header.Length);
            string identifier = "Jebediah Kerman!!!";
            var encoder = new UTF8Encoding (false, true);
            byte[] identifierBytes = encoder.GetBytes (identifier);
            Array.Copy (identifierBytes, 0, helloMessage, header.Length, identifierBytes.Length);
        }

        [Test]
        public void ValidHelloMessageWithNoIdentifier ()
        {
            for (int i = 8; i < helloMessage.Length; i++) {
                helloMessage [i] = 0x00;
            }
            var stream = new TestStream (new MemoryStream (helloMessage));

            // Create mock byte server and client
            var mockByteServer = new Mock<IServer<byte,byte>> ();
            var byteServer = mockByteServer.Object;
            var byteClient = new TestClient (stream);

            var server = new RPCServer (byteServer);
            server.OnClientRequestingConnection += (sender, e) => e.Request.Allow ();
            server.Start ();

            // Fire a client connection event
            var eventArgs = new ClientRequestingConnectionArgs<byte,byte> (byteClient);
            mockByteServer.Raise (m => m.OnClientRequestingConnection += null, eventArgs);

            Assert.IsTrue (eventArgs.Request.ShouldAllow);
            Assert.IsFalse (eventArgs.Request.ShouldDeny);

            server.Update ();
            Assert.AreEqual (1, server.Clients.Count ());
            Assert.AreEqual ("", server.Clients.First ().Name);
        }

        [Test]
        public void ValidHelloMessage ()
        {
            var stream = new TestStream (new MemoryStream (helloMessage));

            // Create mock byte server and client
            var mockByteServer = new Mock<IServer<byte,byte>> ();
            var byteServer = mockByteServer.Object;
            var byteClient = new TestClient (stream);

            var server = new RPCServer (byteServer);
            server.OnClientRequestingConnection += (sender, e) => e.Request.Allow ();
            server.Start ();

            // Fire a client connection event
            var eventArgs = new ClientRequestingConnectionArgs<byte,byte> (byteClient);
            mockByteServer.Raise (m => m.OnClientRequestingConnection += null, eventArgs);

            Assert.IsTrue (eventArgs.Request.ShouldAllow);
            Assert.IsFalse (eventArgs.Request.ShouldDeny);

            server.Update ();
            Assert.AreEqual (1, server.Clients.Count ());
            Assert.AreEqual ("Jebediah Kerman!!!", server.Clients.First ().Name);
        }

        [Test]
        public void InvalidHelloMessageHeader ()
        {
            helloMessage [4] = 0x42;
            var stream = new TestStream (new MemoryStream (helloMessage));

            // Create mock byte server and client
            var mockByteServer = new Mock<IServer<byte,byte>> ();
            var byteServer = mockByteServer.Object;
            var mockByteClient = new Mock<IClient<byte,byte>> ();
            mockByteClient.Setup (x => x.Stream).Returns (stream);
            var byteClient = mockByteClient.Object;

            var server = new RPCServer (byteServer);
            server.OnClientRequestingConnection += (sender, e) => e.Request.Allow ();
            server.Start ();

            // Fire a client connection event
            var eventArgs = new ClientRequestingConnectionArgs<byte,byte> (byteClient);
            mockByteServer.Raise (m => m.OnClientRequestingConnection += null, eventArgs);

            Assert.IsFalse (eventArgs.Request.ShouldAllow);
            Assert.IsTrue (eventArgs.Request.ShouldDeny);
        }

        [Test]
        public void InvalidHelloMessageIdentifier ()
        {
            helloMessage [15] = 0x00;
            var stream = new TestStream (new MemoryStream (helloMessage));

            // Create mock byte server and client
            var mockByteServer = new Mock<IServer<byte,byte>> ();
            var byteServer = mockByteServer.Object;
            var mockByteClient = new Mock<IClient<byte,byte>> ();
            mockByteClient.Setup (x => x.Stream).Returns (stream);
            var byteClient = mockByteClient.Object;

            var server = new RPCServer (byteServer);
            server.OnClientRequestingConnection += (sender, e) => e.Request.Allow ();
            server.Start ();

            // Fire a client connection event
            var eventArgs = new ClientRequestingConnectionArgs<byte,byte> (byteClient);
            mockByteServer.Raise (m => m.OnClientRequestingConnection += null, eventArgs);

            Assert.IsFalse (eventArgs.Request.ShouldAllow);
            Assert.IsTrue (eventArgs.Request.ShouldDeny);
        }

        [Test]
        public void ShortHelloMessageHeader ()
        {
            var message = new byte[] { 0x48, 0x45, 0x4C };
            var stream = new TestStream (new MemoryStream (message));

            // Create mock byte server and client
            var mockByteServer = new Mock<IServer<byte,byte>> ();
            var byteServer = mockByteServer.Object;
            var mockByteClient = new Mock<IClient<byte,byte>> ();
            mockByteClient.Setup (x => x.Stream).Returns (stream);
            var byteClient = mockByteClient.Object;

            var server = new RPCServer (byteServer);
            server.OnClientRequestingConnection += (sender, e) => e.Request.Allow ();
            server.Start ();

            // Fire a client connection event
            var eventArgs = new ClientRequestingConnectionArgs<byte,byte> (byteClient);
            mockByteServer.Raise (m => m.OnClientRequestingConnection += null, eventArgs);

            Assert.IsFalse (eventArgs.Request.ShouldAllow);
            Assert.IsTrue (eventArgs.Request.ShouldDeny);
        }

        [Test]
        public void ShortHelloMessageIdentifier ()
        {
            var shortHelloMessage = new byte[8 + 31];
            byte[] header = { 0x48, 0x45, 0x4C, 0x4C, 0x4F, 0xBA, 0xDA, 0x55 };
            Array.Copy (header, shortHelloMessage, header.Length);
            string identifier = "Jebediah Kerman!!!";
            var encoder = new UTF8Encoding (false, true);
            byte[] identifierBytes = encoder.GetBytes (identifier);
            Array.Copy (identifierBytes, 0, shortHelloMessage, header.Length, identifierBytes.Length);

            var stream = new TestStream (new MemoryStream (shortHelloMessage));

            // Create mock byte server and client
            var mockByteServer = new Mock<IServer<byte,byte>> ();
            var byteServer = mockByteServer.Object;
            var mockByteClient = new Mock<IClient<byte,byte>> ();
            mockByteClient.Setup (x => x.Stream).Returns (stream);
            var byteClient = mockByteClient.Object;

            var server = new RPCServer (byteServer);
            server.OnClientRequestingConnection += (sender, e) => e.Request.Allow ();
            server.Start ();

            // Fire a client connection event
            var eventArgs = new ClientRequestingConnectionArgs<byte,byte> (byteClient);
            mockByteServer.Raise (m => m.OnClientRequestingConnection += null, eventArgs);

            Assert.IsFalse (eventArgs.Request.ShouldAllow);
            Assert.IsTrue (eventArgs.Request.ShouldDeny);
        }

        [Test]
        public void NoHelloMessage ()
        {
            var stream = new TestStream (new MemoryStream ());

            // Create mock byte server and client
            var mockByteServer = new Mock<IServer<byte,byte>> ();
            var byteServer = mockByteServer.Object;
            var mockByteClient = new Mock<IClient<byte,byte>> ();
            mockByteClient.Setup (x => x.Stream).Returns (stream);
            var byteClient = mockByteClient.Object;

            var server = new RPCServer (byteServer);
            server.OnClientRequestingConnection += (sender, e) => e.Request.Allow ();
            server.Start ();

            // Fire a client connection event
            var eventArgs = new ClientRequestingConnectionArgs<byte,byte> (byteClient);
            mockByteServer.Raise (m => m.OnClientRequestingConnection += null, eventArgs);

            Assert.IsFalse (eventArgs.Request.ShouldAllow);
            Assert.IsTrue (eventArgs.Request.ShouldDeny);
        }
    }
}
