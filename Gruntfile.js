/*
 * grunt-codesign
 * https://github.com/jpoon/grunt-codesign
 *
 * Copyright (c) 2015 Jason Poon
 * Licensed under the MIT license.
 */

'use strict';

module.exports = function(grunt) {

  // Project configuration.
  grunt.initConfig({
    jshint: {
      all: [
        'Gruntfile.js',
        'tasks/*.js',
      ],
      options: {
        jshintrc: '.jshintrc'
      }
    },

    // Before generating any new files, remove any previously-created files.
    clean: {
      tests: ['tmp']
    },

    // Configuration to be run (and then tested).
    codesign: {
      default_options: {
        options: {
          certificateFilePath: '',
          certificatePassword: '',
          certificateSha1: '',
          forceXml: false
        },
        files: {
          src: ['']
        },
      }
    },

    // Unit tests.
    //nodeunit: {
    //  tests: ['test/*_test.js']
    //}

  });

  // Actually load this plugin's task(s).
  grunt.loadTasks('tasks');

  // These plugins provide necessary tasks.
  grunt.loadNpmTasks('grunt-contrib-jshint');
  grunt.loadNpmTasks('grunt-contrib-clean');
  grunt.loadNpmTasks('grunt-contrib-nodeunit');

  // Whenever the "test" task is run, first clean the "tmp" dir, then run this
  // plugin's task(s), then test the result.
  //grunt.registerTask('test', ['clean', 'codesign', 'nodeunit']);

  // By default, lint and run all tests.
  grunt.registerTask('default', ['jshint']);

};
