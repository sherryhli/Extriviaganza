# Extriviaganza

A (big) work in progress.

## Introduction

Extriviaganza is a project inspired by Protobowl, an online quizbowl game that I have enjoyed playing in the past. Credit goes to the creators of Protobowl for the game idea but all source code here is my own work. The goal is to create a high-performance and reliable application using a new combination of tools and frameworks. This will be the largest personal project I've ever done so I'm excited for the challenges ahead.

## Project Details

The project is still in early stages but I envision several different components:

### pdfToTxt Script

Questions are obtained from packets found at [the packet archive](http://quizbowlpackets.com/), which are in `.pdf` formats. The first step is to convert `.pdf` files into `.txt` files using a Python command line tool provided by PDFMiner (`pdf2txt.py`). I have created a simple Powershell script that calls the PDFMiner tool and organizes output files.

### QbPackParser Console App

This C# console app may be better named `QbPackParserAndDbLoader`, as the intention is to parse quizbowl questions into JSON objects like the one below, which represents a single question. The questions will then be loaded into a SQL database using a simple web API that accepts bulk `POST` requests.

```
{
    "id": 1,
    "level": "",
    "tournament": "",
    "year": 2019,
    "bonus": "",
    "body": "",
    "answer": "",
    "notes": ""
}
```

### QbQuestions REST API

This will be a simple .NET Core API supporting CRUD operations that interacts with a database storing all quizbowl questions to be used in the game. Entity Framework Core will be used as the ORM to map models to the database schema. The main purpose of this API is to add questions to the database and to provide questions to the game backend.

### Game Backend

The game backend will likely be created using SignalR or WebSockets, which will allow multiple players to interact in real-time when playing the game. If using SignalR, then this component will also be built with .NET Core. WebSockets will allow some other language/framework choices.

### Game Frontend

The frontend will likely be done using Angular or React.

Updates to this README will be made as I work out more details to this project.