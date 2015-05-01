# grunt-codesign [![NPM version](https://badge.fury.io/js/grunt-codesign.svg)](http://badge.fury.io/js/winston-azuretable)

> Code sign files

## Getting Started
This plugin requires Grunt `~0.4.5`

If you haven't used [Grunt](http://gruntjs.com/) before, be sure to check out the [Getting Started](http://gruntjs.com/getting-started) guide, as it explains how to create a [Gruntfile](http://gruntjs.com/sample-gruntfile) as well as install and use Grunt plugins. Once you're familiar with that process, you may install this plugin with this command:

```shell
npm install grunt-codesign --save-dev
```

Once the plugin has been installed, it may be enabled inside your Gruntfile with this line of JavaScript:

```js
grunt.loadNpmTasks('grunt-codesign');
```

## codesign task

### Overview
In your project's Gruntfile, add a section named `codesign` to the data object passed into `grunt.initConfig()`.

```js
grunt.initConfig({
  codesign: {
    options: {
      certificateFilePath: '/path/to/certificate.pfx',
      certificatePassword: 'certificate-password',
      signToolPath: '/path/to/my/local/sign/tool'
    },
    files: {
      src: [
        '/path/to/file-1/to/be/signed',
        '/path/to/file-2/to/be/signed',
      ]
    },
  },
});
```

### Options

#### options.certificateFilePath
Type: `String`
Default value: null

Defines the file path of the certificate to be used for code signing.

#### options.certificatePassword
Type: `String`
Default value: null

Defines the password, if available, of the certificate.

#### options.signToolPath
Type: `String`
Default value: `['C:/Program Files (x86)/Microsoft SDKs/Windows/v7.1A/Bin/signtool.exe', 'C:/Program Files (x86)/Windows Kits/8.0/bin/x86/signtool.exe', 'C:/Program Files (x86)/Windows Kits/8.1/bin/x86/signtool.exe']`

Defines override for path of signtool.exe

## Supported Platforms
* Windows 7+

## Contributing
In lieu of a formal styleguide, take care to maintain the existing coding style. Add unit tests for any new or changed functionality. Lint and test your code using [Grunt](http://gruntjs.com/).
