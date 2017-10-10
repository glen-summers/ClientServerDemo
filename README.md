## Synopsis
CSharp WCF Client\Server SqlServer\MySql DataLayer Archetype\Demo project

## Motivation
Having written many client server projects on the Windows platform it seemed useful to create an archetypal project that encapsulates many standard features and includes some useful utilities and diagnostics

Includes:
* WCF client\server
* Single client executable deployment with Fody/Costura
* Database agnostic middle-tier
* Simple low dependency logging
* WCF inspectors to log parameters and exceptions
* SqlServer and MySql data implementations
* Unit Tests
* Integration Tests
* Code Coverage Report
* Automated Build using MSBuild\VSWhere
* Automated MySql deploy sqript

## Prerequisites
Visual Studio 2017 including .net 4.7, SqlServer, iisexpress
Optional: MqSql Server and .net connector

## Build 
using go.cmd in project root

### Compile and run unit tests
```
> go
```

### Code coverage Report
```
> go coverage
```

### Compile and start server and run integration tests
```
> go full
```

## License
This project is licensed under the terms of the MIT license