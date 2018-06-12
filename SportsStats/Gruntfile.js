module.exports = function (grunt) {

    // Project configuration.
    grunt.initConfig({
        pkg: grunt.file.readJSON('package.json'),
        jshint: {
            options: {
                curly: true,
                eqeqeq: true,
                eqnull: true,
                browser: true,
                globals: {
                    jQuery: true
                },
                predef: ["angular", "sportsApp"]
            },
            all: ['app/**/*.js', 'assets/js/*.js']
        },
        uglify: {
            my_target: {
                files: {
                    'dest/<%= pkg.name %>.min.js': ['app/*.js', 'app/components/**/*.js', 'app/shared/**/*.js', 'assets/js/*.js']
                }
            }
        },
        cssmin: {
            options: {
                shorthandCompacting: false,
                roundingPrecision: -1
            },
            target: {
                files: {
                    'dest/sportsstats.min.css': ['assets/css/sportsstats.css']
                }
            }
        },
        cachebreaker: {
            dev: {
                options: {
                    match: ['assets/css/bootstrap.min.css', 'dest/sportsstats.min.css', 'assets/lib/jquery-1.9.1.min.js', 'assets/lib/bootstrap.min.js', 'assets/lib/angular.min.js', 'assets/lib/angular-route.min.js', 'dest/sportsstats.min.js']
                },
                files: {
                    src: ['index.html']
                }
            }
        }
    });

    // Load the plugin that provides the "uglify" task.
    grunt.loadNpmTasks('grunt-contrib-jshint');
    grunt.loadNpmTasks('grunt-contrib-uglify');
    grunt.loadNpmTasks('grunt-contrib-cssmin');
    grunt.loadNpmTasks('grunt-cache-breaker');

    // Default task(s).
    grunt.registerTask('default', ['jshint', 'uglify', 'cachebreaker', 'cssmin']);

};
