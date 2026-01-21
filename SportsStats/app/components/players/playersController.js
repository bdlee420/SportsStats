(function () {
    "use strict";

    sportsApp.controller('playersController', ['$scope', '$http', '$rootScope', '$location', 'CurrentStateFactory',
        function playersController($scope, $http, $rootScope, $location, CurrentStateFactory) {
            $rootScope.RedirectURL = "Players";
            var currentState;
            $scope.HideNumber = true;
            $rootScope.ShowSpinner = true;
            CurrentStateFactory.getUser().then(function (user) {
                if (user.data == null) {
                    $rootScope.ShowSpinner = false;
                    $location.url("/SportsStats/Login");
                }
                else {
                    currentState = CurrentStateFactory.getState(true, user.data.UserName);
                    if (!currentState) {
                        return;
                    }

                    $scope.ReadOnly = user.data.RoleID !== 1;
                    $http.get("/SportsStats/api/Players").then(function (success) {
                        var data = success.data;
                        $scope.ListPlayers = data;
                        $rootScope.ShowSpinner = false;
                    }, function (error) {
                    });
                }
            });

            $scope.AddPlayer = function (player) {
                var newPlayer = jQuery.extend(true, {}, player);
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Players/AddPlayer', newPlayer).then(function (success) {
                    var data = success.data;
                    $scope.ListPlayers = data;
                    player.Name = null;
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });                   
            };
        }
    ]);

    sportsApp.controller('playerController', ['$scope', '$http', '$routeParams', '$rootScope', '$location', '$cookies', 'CurrentStateFactory',
        function playerController($scope, $http, $routeParams, $rootScope, $location, $cookies, CurrentStateFactory) {
            $rootScope.RedirectURL = "Players/" + $routeParams.playerID;
            $rootScope.ShowSpinner = true;
            CurrentStateFactory.getUser().then(function (user) {
                if (user.data == null) {
                    $rootScope.ShowSpinner = false;
                    $location.url("/SportsStats/Login");
                }
                else {
                    $scope.UserName = user.data.UserName;
                    $scope.ReadOnly = user.data.RoleID !== 1;
                    $http.get("/SportsStats/api/Players/" + $routeParams.playerID).then(function (success) {
                        var data = success.data;
                        $scope.Player = data;
                        new UpdateStatsObject(data.HockeyStatTypes, data.HockeyStats, data.ID, data.ID);
                        new UpdateStatsObject(data.BaseballStatTypes, data.BaseballStats, data.ID, data.ID);
                        new UpdateStatsObject(data.BasketballStatTypes, data.BasketballStats, data.ID, data.ID);
                        new UpdateStatsObject(data.BasketballStatTypes, data.TotalBasketballStats, data.ID, data.ID);
                        $rootScope.ShowSpinner = false;
                    }, function (error) {
                    });  
                }
            });

            $scope.SavePlayer = function (player) {
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Players/SavePlayer', player).then(function (success) {
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.GoToTeam = function (team) {
                $scope.UpdateLeagueSelection(team.SportID, team.LeagueID);
                $location.url("/SportsStats/Team/" + team.ID);
            };

            $scope.UpdateLeagueSelection = function (sportID, leagueID) {
                $rootScope.CurrentState =
                   {
                       SelectedSportID: sportID,
                       SelectedLeagueID: leagueID
                   };
               
                var cookieNameSport = 'sportsstats.com:' + $scope.UserName + ':SelectedSportID';
                var cookieNameLeague = 'sportsstats.com:' + $scope.UserName + ':SelectedLeagueID';
                $cookies.remove(cookieNameSport);
                $cookies.remove(cookieNameLeague);

                var expiration = new Date();
                expiration.setDate(expiration.getDate() + 30);
                $cookies.put(cookieNameSport, sportID, { expires: expiration });
                $cookies.put(cookieNameLeague, leagueID, { expires: expiration });
            };
        }
    ]);

}());