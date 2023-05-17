# Totk Decompression/Compression Tool

[![License: AGPL v3](https://img.shields.io/badge/License-AGPL_v3-blue.svg)](https://www.gnu.org/licenses/agpl-3.0)

A simple tool for decompressing and compressing `.zs` files in Totk.

![Totk Decompression/Compression Tool](https://user-images.githubusercontent.com/80713508/235798842-421d9487-8bc1-47cd-920e-9a9f147bcf1f.png)

## Table of Contents
- [Setup](#setup)
- [Usage](#usage)
- [Command Line Usage](#command-line-usage)
- [License](#license)

## Setup

1. Download and install the .NET 7 Runtime.
2. Download the latest release of [Totk zStd Tool](https://github.com/TotkMods/Totk.ZStdTool/releases/latest).
3. Open the application and configure your settings:
   - **Game Path**: Specify the path to your Totk game dump. This field is required.

## Usage

To use the Totk Decompression/Compression Tool, follow these steps:

1. Browse for a `.zs` file located in your Totk game dump.
2. Click on the `Decompress` button to decompress the file and save the raw output to your computer.

## Command Line Usage

> **Note:** Before using the command line interface (CLI) tools, make sure you have set up the game paths in the user interface.

- To decompress a file, use the following command: `decompress </path/file.bin.zs> [-o|--output OUTPUT]`

   Example:
   ```
   decompress F:\Bin\Totk\Bootup.pack.zs -o ./Bootup.pack
   ```

- To compress a file, use the following command: `compress </path/file.bin> [-o|--output OUTPUT]`

   Example:
   ```
   compress F:\Bin\Totk\Bootup.pack -o ./Bootup.pack.zs
   ```

## License

This project is licensed under the [AGPL v3 License](https://www.gnu.org/licenses/agpl-3.0).

&copy; Arch Leaders - 2023
