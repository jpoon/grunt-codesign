/*
 * grunt-codesign
 * https://github.com/jpoon/grunt-codesign
 *
 * Copyright (c) 2015 Jason Poon
 * Licensed under the MIT license.
 */

'use strict';

module.exports = function(grunt) {

  grunt.registerMultiTask('codesign', 'CodeSign Files', function() {
    var options = this.options({
      signToolPath: [
        'C:/Program Files (x86)/Microsoft SDKs/Windows/v7.1A/Bin/signtool.exe',
        'C:/Program Files (x86)/Windows Kits/8.0/bin/x86/signtool.exe',
        'C:/Program Files (x86)/Windows Kits/8.1/bin/x86/signtool.exe'
      ]
    });

    this.requiresConfig(this.certificateFilePath);

    var cmd, args;
    switch(process.platform) {
      case 'win32':
        var cmd;

        grunt.verbose.writeln('Searching for signtool...')
        options.signToolPath = options.signToolPath instanceof Array ? options.signToolPath : [options.signToolPath];
        options.signToolPath.every(function(path) {
          grunt.verbose.write(path)

          if (grunt.file.exists(path)) {
            grunt.verbose.writeln(' found')
            cmd = path;
            return false;
          }

          grunt.verbose.writeln(' not found')
          return true;
        });

        if (!cmd) {
          grunt.fail.fatal('Unable to find signtool.exe. Ensure Windows SDK installed')
        }


        args = ['sign'];

        // sign with cert file path?
        if (options.certificateFilePath) {
          args.push('/f', options.certificateFilePath);
          // certificate password
          if (options.certificatePassword) {
            args.push('/p', options.certificatePassword);
          }
        }
        else if (options.certificateSha1) {
          // Use SHA1 thumbprint (the cert should be installed in the store)
          args.push('/sha1', options.certificateSha1);
        }
        // verbose
        args.push('/v');
        break;
      default:
        grunt.fail.fatal('Unsupported platform: ' + process.platform);
    }

    var done = this.async();
    var callback = function(error, result, code) {
      grunt.verbose.writeln(result);
      if (code !== 0) {
        grunt.fail.warn(error);
      }
      done();
    };

    this.filesSrc.forEach(function(file) {
      grunt.log.ok("signing " + file);
      grunt.util.spawn({
        cmd: cmd,
        args: args.concat(file)
      }, callback);
    });

  });
};
