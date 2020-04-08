using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace WebSocketDemo
{
    public class WebSocketHandler
    {
        public async Task ListenerWebSocketRequest(HttpContext context, WebSocket webSocket) //Echo 响应的意思
        {
            while (true)
            {
                var buffer = new byte[1024 * 4];
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.CloseStatus.HasValue)
                {
                    await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                    break;
                }
                Task.Run(()=> { EchoRequest(webSocket, buffer); });

            }
        }

        private void EchoRequest(WebSocket webSocket, byte[] buffer)
        {
            //接收数据
            string receiveText = System.Text.Encoding.Default.GetString(buffer);

            Thread.Sleep(10000);

            //发送消息接收确认
            byte[] sendBuffer = System.Text.Encoding.Default.GetBytes($"服务端已收到数据:{receiveText}");
            //await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
            webSocket.SendAsync(new ArraySegment<byte>(sendBuffer, 0, sendBuffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);

            //发送消息处理结果
            string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sendBuffer = System.Text.Encoding.Default.GetBytes(s);
            webSocket.SendAsync(new ArraySegment<byte>(sendBuffer, 0, sendBuffer.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}
