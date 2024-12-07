(function () {
    "use strict";

    sportsApp.controller('loginController', ['$scope', '$http', '$rootScope', '$location', '$routeParams', '$cookies',
        function playersController($scope, $http, $rootScope, $location, $routeParams, $cookies) {
            $rootScope.LoginText = "Login";
            $scope.InvalidLogin = false;
            var user2 = {
                UserName: $routeParams.user,
                Password: "nopassword",
                RequestedGameID: $routeParams.gameId
            };       

            $scope.UpdateLeagueSelection = function (selection) {
                $rootScope.CurrentState =
                {
                    SelectedSportID: selection.SportID,
                    SelectedLeagueID: selection.LeagueID
                };

                var expiration = new Date();
                expiration.setDate(expiration.getDate() + 30);
                $cookies.put('sportsstats.com:' + $scope.UserName + ':SelectedSportID', selection.SportID, { expires: expiration });
                $cookies.put('sportsstats.com:' + $scope.UserName + ':SelectedLeagueID', selection.LeagueID, { expires: expiration });
            };

            $scope.Login = function (user) {
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Login/LoginUser', user).then(function (success) {
                    $rootScope.ShowSpinner = false;
                    var loginRes = success.data;
                    if (loginRes > -1) {            
                        if (loginRes > 0) {
                            $scope.UpdateLeagueSelection({ SportID: 1, LeagueID: loginRes });
                        }
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