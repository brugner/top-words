# Top Words

This little app was made using [ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-2.2 "ASP.NET Core"). It consists of a web API documented with [Swagger](https://swagger.io/ "Swagger") (you can check it [here](http://topwords.azurewebsites.net "here")) and a simple [Razor page](https://docs.microsoft.com/en-us/aspnet/core/razor-pages/?view=aspnetcore-2.2&tabs=visual-studio "Razor page") with [Bootstrap 4](https://getbootstrap.com/ "Bootstrap 4").
The key here was to learn two technologies new to me. On the one hand, how a [web crawler](https://en.wikipedia.org/wiki/Web_crawler "web crawler") work. In this case I'm using [Abot](https://github.com/sjdirect/abot "Abot"). On the other hand, I wanted to implement notifications from the server to the client (web socket). For this, I used [SignalR](https://docs.microsoft.com/en-us/aspnet/core/signalr/introduction?view=aspnetcore-2.2 "SignalR").

I draw inspiration from [this](https://loudwire.com/most-commonly-used-word-slipknot-lyrics/ "this") article.

You can find the sorce code on [Github](https://github.com/n6ither/top-words "Github").

The application **does not** save the lyrics anywhere. It's just a very basic algorithm that counts words frequency.
