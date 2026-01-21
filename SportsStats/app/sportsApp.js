/*global sportsApp: true*/
var sportsApp = angular.module('sportsApp', ['ngCookies', 'ngRoute', 'ngTouch']);

sportsApp.config(['$routeProvider', '$locationProvider',
    function ($routeProvider, $locationProvider) {
        $locationProvider.html5Mode({
            enabled: true,
            requireBase: false
        });
        $routeProvider.
            when('/SportsStats/Login', {
                templateUrl: '/SportsStats/app/components/login/login.html',
            }).
            when('/SportsStats/League', {
                templateUrl: '/SportsStats/app/components/league/league.html',
            }).
            when('/SportsStats/Team/:teamID', {
                templateUrl: '/SportsStats/app/components/team/team.html?version=1',
            }).
            when('/SportsStats/Team', {
                templateUrl: '/SportsStats/app/components/team/team.html?version=1',
            }).
            when('/SportsStats/Games', {
                templateUrl: '/SportsStats/app/components/games/games.html',
            }).
            when('/SportsStats/Games/:gameID', {
                templateUrl: '/SportsStats/app/components/games/game.html',
            }).
            when('/SportsStats/GameLog/:gameID', {
                templateUrl: '/SportsStats/app/components/gamelog/gamelog.html',
            }).
            when('/SportsStats/Players', {
                templateUrl: '/SportsStats/app/components/players/players.html',
            }).
            when('/SportsStats/Players/:playerID', {
                templateUrl: '/SportsStats/app/components/players/player.html?version=2',
            }).
            when('/SportsStats/Stats/:gameID/:teamID/:playerID', {
                templateUrl: '/SportsStats/app/components/stats/stat.html',
            }).
            otherwise({
                templateUrl: '/SportsStats/app/components/leagues/leagues.html',
            });
    }]);

sportsApp.filter('picker', ['$filter', function ($filter) {
    return function (value, filterName) {
        if (filterName.length === 0) {
            return $filter('number')(value);
        }

        var splitArray = filterName.split(":");
        var filterName2 = splitArray[0];
        var value2 = splitArray[1];
        if (filterName2 === "percentage") {
            return $filter('number')(value * 100, value2) + '%';
        }
        else if (filterName2 === "decimal") {
            return $filter('number')(value, value2);
        }
        return $filter(filterName)(value);
    };
}]);

sportsApp.run(['$rootScope', '$location', '$anchorScroll', '$routeParams', function ($rootScope, $location, $anchorScroll, $routeParams) {
    $rootScope.$on('$routeChangeSuccess', function (newRoute, oldRoute) {
        $location.hash($routeParams.scrollTo);
        $anchorScroll();
    });
}]);

angular.module("ngTouch", [])
    .directive("ngTouchstart", function () {
        return {
            controller: ["$scope", "$element", function ($scope, $element) {

                $element.bind("touchstart", onTouchStart);
                function onTouchStart(event) {
                    var method = $element.attr("ng-touchstart");
                    $scope.$event = event;
                    $scope.$apply(method);
                }

            }]
        };
    })
    .directive("ngTouchmove", function () {
        return {
            controller: ["$scope", "$element", function ($scope, $element) {

                $element.bind("touchmove", onTouchMove);

                function onTouchMove(event) {
                    var method = $element.attr("ng-touchmove");
                    $scope.$event = event;
                    $scope.$apply(method);
                }

            }]
        };
    })
    .directive("ngTouchend", function () {
        return {
            controller: ["$scope", "$element", function ($scope, $element) {

                $element.bind("touchend", onTouchEnd);
                function onTouchEnd(event) {
                    var method = $element.attr("ng-touchend");
                    $scope.$event = event;
                    $scope.$apply(method);
                }

            }]
        };
    });

sportsApp.factory('CurrentStateFactory', ['$rootScope', '$cookies', '$location', '$http', function ($rootScope, $cookies, $location, $http) {
    var service = {};

    service.getState = function (redirect, userName) {
        if (!$rootScope.CurrentState) {
            var cookie_SelectedSportID = parseInt($cookies.get('sportsstats.com:' + userName + ':SelectedSportID'));
            var cookie_SelectedLeagueID = parseInt($cookies.get('sportsstats.com:' + userName + ':SelectedLeagueID'));
            var cookie_SelectedTeamID = parseInt($cookies.get('sportsstats.com:' + userName + ':SelectedTeamID'));

            $rootScope.CurrentState =
            {
                SelectedSportID: 0,
                SelectedLeagueID: 0,
                SelectedTeamID: 0
            };

            if (typeof cookie_SelectedSportID !== 'undefined' && !isNaN(parseFloat(cookie_SelectedSportID))) {
                $rootScope.CurrentState.SelectedSportID = cookie_SelectedSportID;
            }

            if (typeof cookie_SelectedLeagueID !== 'undefined' && !isNaN(parseFloat(cookie_SelectedLeagueID))) {
                $rootScope.CurrentState.SelectedLeagueID = cookie_SelectedLeagueID;
            }

            if (typeof cookie_SelectedTeamID !== 'undefined' && !isNaN(parseFloat(cookie_SelectedTeamID))) {
                $rootScope.CurrentState.SelectedTeamID = cookie_SelectedTeamID;
            }
        }
        if (redirect && ($rootScope.CurrentState.SelectedLeagueID === 0 || $rootScope.CurrentState.SelectedSportID === 0 || $rootScope.CurrentState.SelectedTeamID === 0)) {
            $location.url("/SportsStats");
            return false;
        }
        else {
            return $rootScope.CurrentState;
        }
    };


    service.getUser = function () {
        return $http.get("/SportsStats/api/Login/GetUser").then(function (success) {
            if (success.data != null) {
                $rootScope.HasOneTeam = success.data.HasOneTeam;
                $rootScope.LoginText = "Logout";
            }
            return success;
        }, function (error) { });
    };

    return service;
}]);