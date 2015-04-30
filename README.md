# grunt-codesign

> Codesigns files

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
```

## Supported Platforms
* Windows

## Contributing
In lieu of a formal styleguide, take care to maintain the existing coding style. Add unit tests for any new or changed functionality. Lint and test your code using [Grunt](http://gruntjs.com/).