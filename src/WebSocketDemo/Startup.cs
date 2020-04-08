using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace WebSocketDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.WebSockets.IsWebSocketRequest)
                {
                    using (IServiceScope scope = app.ApplicationServices.CreateScope())
                    {
                        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();//The AcceptWebSocketAsync method upgrades the TCP connection to a WebSocket connection and provides a WebSocket object. Use the WebSocket object to send and receive messages
                        //await ListenerWebSocketRequest(context, webSocket);

                        WebSocketHandler handler = new WebSocketHandler();
                        await handler.ListenerWebSocketRequest(context, webSocket);
                    }
                }
                else
                {
                    //Hand over to the next middleware
                    await next();
                }
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //private async Task ListenerWebSocketRequest(HttpContext context, WebSocket webSocket) //Echo 响应的意思
        //{                        
        //    while (true)
        //    {
        //        var buffer = new byte[1024 * 4];
        //        WebSocketReceiveResult result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
        //        if (result.CloseStatus.HasValue) 
        //        {
        //            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        //            break; 
        //        }

        //        DoRequest(webSocket, buffer);

        //    }
        //}

        //private async Task DoRequest(WebSocket webSocket, byte[] buffer)
        //{
        //    //接收数据
        //    string receiveText = System.Text.Encoding.Default.GetString(buffer);

        //    Thread.Sleep(5000);

        //    //发送消息接收确认
        //    byte[] sendContext = System.Text.Encoding.Default.GetBytes($"服务端已收到数据:{receiveText}");
        //    //await webSocket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
        //    await webSocket.SendAsync(new ArraySegment<byte>(sendContext, 0, sendContext.Length), WebSocketMessageType.Text, true, CancellationToken.None);

        //    //发送消息处理结果
        //    string s = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    sendContext = System.Text.Encoding.Default.GetBytes(s);
        //    await webSocket.SendAsync(new ArraySegment<byte>(sendContext, 0, sendContext.Length), WebSocketMessageType.Text, true, CancellationToken.None);
        //}
    }
}
