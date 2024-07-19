# nanocom

 A simple serial port communication tool that I use to communicate with my Arduino boards. 

 ## Usage:
```shell
  nanocom [command] [options]
```

## Options:
```shell
  --version       Show version information
  -?, -h, --help  Show help and usage information
```

## Commands:
```shell
  open  Open a port for communication
  list  List all available ports
```


## Open options
```shell
  -p, --port <port> (REQUIRED)  The port name
  -b, --baudrate <baudrate>     The baud rate [default: 9600]
  --databits <databits>         The data bits [default: 8]
  --parity <E|N|O>              The parity [default: N]
  --stopbits <1|2>              The stop bits [default: 1]
  -?, -h, --help                Show help and usage information
```

# License
See the LICENSE.md file for license rights and limitations (MIT).
