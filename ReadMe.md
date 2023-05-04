# Totk Decompression Tool

[![License: AGPL v3](https://img.shields.io/badge/License-AGPL_v3-blue.svg)](https://www.gnu.org/licenses/agpl-3.0)

A simple tool to decompress `.zs` files in Totk, compression support is also planned for the future

<img src="https://user-images.githubusercontent.com/80713508/235798842-421d9487-8bc1-47cd-920e-9a9f147bcf1f.png" width="500">


## Setup

- Download the .NET 7 Runtime
- Download the latest release of [Totk zStd Tool](https://github.com/TotkMods/Totk.ZStdTool/releases/latest)
- Open the app and configure your settings
  - Game Path: The path to your Totk game dump *(required)*

## Usage

- Browse for a `.zs` file in your Totk game dump
- Click `Decompress` and save the raw file to your computer

## Usage (CLI)
- To use the CLI tools you must first setup the game paths in the user-interface.
- Decompress `Totk.ZStdTool.exe -x Something.zs`
  - The output will be in the same folder but without `.zs` extension
- Compress `Totk.ZStdTool.exe -c Something.bin`
  - The output will be in the same folder but with `.zs.new` extension

<br>

*(c) Arch Leaders - 2023*
