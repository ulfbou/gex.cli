# Build Props

This directory contains additional MSBuild property files that can be imported for specific build scenarios.

## Available Props

Currently, shared build properties are defined in `Directory.Build.props` at the solution root level.

## Adding Custom Props

You can add custom `.props` files here for:

- Specific build configurations
- Platform-specific settings
- Team-specific conventions
- Conditional package references

## Importing Custom Props

To use a custom props file in a project:

```xml
<Import Project="$(MSBuildThisFileDirectory)..\..\props\YourCustom.props" />
```

## Standard Props

The main build configuration is in `../Directory.Build.props` which includes:

- Target framework settings (net8.0, net10.0)
- Nullable reference types
- AOT and trimming support
- Documentation generation
- Version information
- Package metadata
