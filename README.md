# Busybody

By Adrian Withy  

## Summary ##

Busybody is the beginnings of a distributed network monitoring system in the likes of SCOM and Nagios.  In it's current form, Busybody runs as a Console Application or Windows Service.


Why another network monitor?  Because I need one for a few personal network projects, and I felt like building one for fun.

The Busybody service is written in C# .NET 4.5, and uses Angular for it's web portal.


## Capabilities ##

Not much yet.  It will just run a ping test on a number of hosts and e-mail alerts on host state changes.

Features:

- Periodically ping test any number of hosts
- Ping test configurable by number of pings, number of failures, and timeout period
- JSON based configuration
- E-mail alerts
- Web monitoring portal self-hosted using OWIN
- Run as console app or install as Windows service
- Configurable number of allowable test failures
- Alerts on host state changes and system errors
- System status web and text report
- Service host CPU and Busybody process RAM usage monitoring
- Info, debug, and trace level text logging
- Text file error reports


## Building ##

Build and run all tests:  
`> .\build.cmd`

Build and package into .\build directory:  
`> .\build.cmd package`


## Installing as Service ##

Installing:  
`> .\busybody.exe install`  (from admin console)

Uninstalling:  
`> .\busybody.exe uninstall`  (from admin console)


## Command Line Args ##

Verbose console logging: `-v`

Specify configuration file path: `-c:C:\SomeDirectory\MyConfiguration.cfg`


## Tests ##

**Available**

* Ping

**To-Do**

* HTTP/HTTPS
* Windows host memory/disk
* Linux host memory/disk
* SSH
* OpenVPN
* FTP/SFTP
* SNMP
* DNS


## Sample Config File ##

    {
		"DataDirectory":"C:\\Busybody\\Data",
		"PollingInterval":300,
		"ListeningUrls":"http://localhost:9000;http://hostname:9000",
		"EmailAlertConfiguration":
		{
			"Enabled":true,
			"FromAddress":"adrian@adrianwithy.com",
			"ToEmailAddress":"adrian@adrianwithy.com",
			"Host":"%SMTP_HOSTNAME%",
			"Port":587,
			"Password":"%PASSWORD%",
		},
		"Hosts":[
		{
			"Nickname":"Local Machine",
			"Hostname":"127.0.0.1",
			"Location":"Server Room",
			"Tests":[
			{
				"Name":"Ping",
				"AllowableFailures":0,
				"Parameters":{
					"TimeoutMs":"2000",
					"Count":"5",
					"MaxFailures":"1",
					"DelayMs":"500",
				}
			}]
		}]
	}



Notes: 

- Polling interval is in seconds
- Test AllowableFailures property is option
- Ping test parameters are optional
- If no ListeningUrls specified, defaults to http://localhost:9000
- You can use a root level "WebRoot" property to specify an override for the web app - for development

## Immediate To-Do ##

1. HTTP/HTTPS test
1. OpenVPN test
1. SSH test
1. Multiple e-mail alerting


## Ideas ##

- Gulp Gulp
- Bower
- Web portal UI testing (Jasmine?, Karma?)
- Weekly e-mailed status reports
- PowerShell based test specification
- Local event store with history
- Web portal for configuration
- Command line configuration wizard
- Automated updates
- Windows agent
- Availability (distributed)
- Windows system tray monitoring app
- Azure hosted service extension
- Azure storage event store
- Amazon S3 event store
- Sites/network namespacing
- Pingdom integration
- Linux agent
- Linux core (Mono)
- Android app
- Installer

## Misc. Notes ##

- Must be run with admin rights when run from the command line with externally available web server URL (i.e., anything but localhost)


## License ##

Apache 2.0 - See LICENSE.txt

No warranty is provided whatsoever.
