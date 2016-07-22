![Icon](http://i.imgur.com/WnKfKOC.png?1) 
# Nanophone [![Build status](https://ci.appveyor.com/api/projects/status/hwk6g88wm7orvcog?svg=true)](https://ci.appveyor.com/project/lvermeulen/nanophone) [![license](https://img.shields.io/github/license/lvermeulen/Nanophone.svg?maxAge=2592000)](https://github.com/lvermeulen/Nanophone/blob/master/LICENSE) [![NuGet](https://img.shields.io/nuget/vpre/Nanophone.Core.svg?maxAge=2592000)](https://www.nuget.org/packages/Nanophone.Core/) [![Join the chat at https://gitter.im/lvermeulen/Nanophone](https://badges.gitter.im/lvermeulen/Nanophone.svg)](https://gitter.im/lvermeulen/Nanophone?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge) ![](https://img.shields.io/badge/.net-4.5.1-yellowgreen.svg) ![](https://img.shields.io/badge/netstandard-1.6-yellowgreen.svg)
Nanophone is a minimalistic library for Service Registration and Discovery and is the driving force behind the [Equalizer](https://github.com/lvermeulen/Equalizer) middleware for aspnetcore.

##Features:
* Find available service instances by service name
* Find available service instances by service name and version
* Extensible service registry host - includes [Consul](https://www.consul.io/) host
* Extensible service registry tenants - includes [Nancy](https://github.com/NancyFx/Nancy) and Web Api tenants
* Supports [eBay Fabio](https://github.com/eBay/fabio) (experimental)

##Usage:

* Find available service instances by service name:
~~~~
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;

var serviceRegistry = new ServiceRegistry();
serviceRegistry.StartClient(new ConsulRegistryHost());

var instances = serviceRegistry.FindServiceInstancesAsync("my-service-name").Result;
foreach (var instance in instances)
{
    Console.WriteLine($"Address: {instance.Address}:{instance.Port}, Version: {instance.Version}");
}
~~~~

* Find available service instances by service name and version:
~~~~
using System.Threading.Tasks;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;

var serviceRegistry = new ServiceRegistry();
serviceRegistry.StartClient(new ConsulRegistryHost());

var instances = serviceRegistry.FindServiceInstancesWithVersionAsync("my-service-name", "1.2").Result;
foreach (var instance in instances)
{
    Console.WriteLine($"Address: {instance.Address}:{instance.Port}, Version: {instance.Version}");
}
~~~~

* Start Nancy service:
~~~~
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.Nancy;

var serviceRegistry = new ServiceRegistry();
serviceRegistry.Start(new NancyRegistryTenant(new Uri("http://localhost:9001")), new ConsulRegistryHost(),
    "customers", "v1");
~~~~

* Start Web Api service:
~~~~
using Microsoft.Owin.Hosting;
using Nanophone.Core;
using Nanophone.RegistryHost.ConsulRegistry;
using Nanophone.RegistryTenant.WebApi;

string url = "http://localhost:9000/";

var serviceRegistry = new ServiceRegistry();
serviceRegistry.Start(new WebApiRegistryTenant(new Uri(url)), new ConsulRegistryHost(), 
    "date", "1.7-pre");

WebApp.Start<Startup>(url);
~~~~

##Thanks
* [SIM Card](https://thenounproject.com/term/sim-card/15160) icon by misirlou from [The Noun Project](https://thenounproject.com)
* Look, up in the sky. It's a bird. It's a plane. It's [LibLog](https://www.nuget.org/packages/LibLog/)!
