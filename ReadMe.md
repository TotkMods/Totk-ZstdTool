# Totk Decompression/Compression Tool

[![License: AGPL v3](https://img.shields.io/badge/License-AGPL_v3-blue.svg)](https://www.gnu.org/licenses/agpl-3.0)

A simple tool to decompress and compress `.zs` files in Totk.

<img src="https://user-images.githubusercontent.com/80713508/235798842-421d9487-8bc1-47cd-920e-9a9f147bcf1f.png" width="500">


## Setup

- Download the .NET 7 Runtime
- Download the latest release of [Totk zStd Tool](https://github.com/TotkMods/Totk.ZStdTool/releases/latest)
- Open the app and configure your settings
  - Game Path: The path to your Totk game dump *(required)*

## Usage

- Browse for a `.zs` file in your Totk game dump
- Click `Decompress` and save the raw file to your computer

## Command Line Usage
> **Notice:** To use the CLI tools you must first setup the game paths in the user interface

- Decompress `d, decompress </path/file.bin.zs> [-o|--output OUTPUT]`<br>
  ```
  decompress F:\Bin\Totk\Bootup.pack.zs -o ./Bootup.pack
  ```

- Compress `c, compress </path/file.bin> [-o|--output OUTPUT]`<br>
  ```
  compress F:\Bin\Totk\Bootup.pack -o ./Bootup.pack.zs
  ```

<br>

*(c) Arch Leaders - 2023*
