(function () {
    "use strict";

    sportsApp.controller('loginController', ['$scope', '$http', '$rootScope', '$location', '$routeParams', '$cookies',
        function playersController($scope, $http, $rootScope, $location, $routeParams, $cookies) {
            $rootScope.LoginText = "Login";
            $scope.InvalidLogin = false;
            var user2 = {
                UserName: $routeParams.user,
                Password: "nopassword"
            };          

            $scope.Login = function (user) {
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Login/LoginUser', user).then(function (success) {
                    $rootScope.ShowSpinner = false;
                    if (success.data) {
                        $rootScope.LoginText = "Logout";
                        $scope.InvalidLogin = false;
                        if ($rootScope.RedirectURL != null) {
                            $location.url("/SportsStats/" + $rootScope.RedirectURL);
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

            if ($routeParams.user != null) {
                $scope.Login(user2);
            }
            else {
                $rootScope.CurrentState = null;        
                $cookies.remove("username");
                $http.get("/SportsStats/api/Login/Logout").then(function (success) {
                }, function (error) {
                });
            }
        }
    ]);
    
}());