# JSON Converters

![GitHub Workflow Status](https://img.shields.io/github/workflow/status/Penca53/json-converters/.NET)
![GitHub](https://img.shields.io/github/license/Penca53/json-converters)
![Nuget](https://img.shields.io/nuget/v/Penca53.JsonConverters)

This libray provides a set of custom `System.Text.Json.Serialization` `JsonConverter` attributes. By default, in `System.Text.Json` it's impossible to serialize private properties, but with these custom converters you will be able to serialize them by just decorating the class you want to serialize with a specific custom converter. 

## NonPublicConverter

Serializes all public and non-public properties.
