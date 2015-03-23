# Busybody

By Adrian Withy  
Last Updated 22 March 2015

## Summary ##

Busybody is the beginnings of a distributed network monitoring system in the likes of SCOM and Nagios.  In it's current form, Busybody runs as a Console Application or Windows Service.


Why another network monitor?  Because I need one for a few personal network projects, and I felt like building one for fun.

Busybody is written in C# .NET 4.5.


## Capabilities ##

Almost none.  It will just run a simple ping test on a number of hosts and write state changes to file.


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
		"PollingInterval":10,
		"Hosts":[
		{
			"Nickname":"Local Machine",
			"Hostname":"127.0.0.1",
			"Tests":[
			{
				"Name":"Ping",
				"Parameters":{
					"TimeoutMs":"500",
					"Count":"5",
					"MaxFailures":"0",
					"DelayMs":"500",
				}
			}]
		}]
	}



Note: Polling interval is in seconds.


## Immediate To-Do ##

1. Asyncify
1. Write simple static html status page
1. E-mail alerting
1. Error handling/reporting/alerting
1. HTTP/HTTPS test
1. OpenVPN test
1. SSH test
1. Busybody process memory monitoring


## Ideas ##

- Local event store with history
- Web portal for status
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


## License ##

Apache 2.0 - See LICENSE.txt

No warranty is provided whatsoever.