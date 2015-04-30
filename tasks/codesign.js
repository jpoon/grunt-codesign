/*
 * grunt-codesign
 * https://github.com/jpoon/grunt-codesign
 *
 * Copyright (c) 2015 Jason Poon
 * Licensed under the MIT license.
 */

'use strict';

module.exports = function(grunt) {

  grunt.registerMultiTask('codesign', 'Grunt CodeSign', function() {
    var options = this.options({});
    this.requiresConfig(this.certificateFilePath);

    var cmd, args;
    switch(process.platform) {
      case 'win32':
        cmd = './bin/signtool';
        args = ['sign'];

        // signing cert file path
        args.push('/f', options.certificateFilePath);
        // verbose
        args.push('/v');
        // certificate password
        if (options.certificatePassword) {
          args.push('/p', options.certificatePassword);
        }
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
