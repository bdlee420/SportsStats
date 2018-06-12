(function () {
    "use strict";

    sportsApp.controller('loginController', ['$scope', '$http', '$rootScope', '$location', '$routeParams', '$cookies',
        function playersController($scope, $http, $rootScope, $location, $routeParams, $cookies) {
            $rootScope.LoginText = "Login";
            $scope.InvalidLogin = false;

            $cookies.remove("username");
            $rootScope.CurrentState = null;

            $http.get("/SportsStats/api/Login/Logout").then(function (success) {                
            }, function (error) {
            });

            $scope.Login = function (user) {
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Login/LoginUser', user).then(function (success) {
                    $rootScope.ShowSpinner = false;
                    if (success.data) {
                        $rootScope.LoginText = "Logout";
                        $scope.InvalidLogin = false;
                        if (typeof $routeParams.redirect !== 'undefined' && $routeParams.redirect.length > 0) {
                            $location.url("/SportsStats/" + $routeParams.redirect);
                        }
                        else {
                            $location.url("/SportsStats/Sports");
                        }
                    }
                    else
                    {
                        $scope.InvalidLogin = true;
                    }
                }, function (error) {
                });                   
            };
        }
    ]);
    
}());