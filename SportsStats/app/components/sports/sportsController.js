(function () {
    "use strict";

    sportsApp.controller('sportsController', ['$scope', '$routeParams', '$rootScope', '$http', '$cookies', '$location', 'CurrentStateFactory',
        function sportsController($scope, $routeParams, $rootScope, $http, $cookies, $location, CurrentStateFactory) {
            var currentState;
            $rootScope.ShowSpinner = true;

            CurrentStateFactory.getUser().then(function (user) {
                if (user.data == null || $routeParams.user != null) {
                    $rootScope.ShowSpinner = false;
                    $location.url("/SportsStats/Login?user=" + $routeParams.user);
                }
                else {                    
                    $scope.UserName = user.data.UserName;
                    if (user.data.HasOneLeague && user.data.HasOneTeam) {
                        var sportID = user.data.Sports[0];
                        var leagueID = user.data.Leagues[0];
                        $scope.UpdateLeagueSelection({ SportID: sportID, LeagueID: leagueID });
                        if ($rootScope.RedirectURL != null) {
                            $location.url("/SportsStats/" + $rootScope.RedirectURL);
                        }
                        else {
                            $location.url("/SportsStats/Teams");
                        }
                    }
                    else {
                        currentState = CurrentStateFactory.getState(false, $scope.UserName);
                        $scope.SelectedSportID = currentState.SelectedSportID;
                        $scope.SelectedLeagueID = currentState.SelectedLeagueID;
                        $http.get("/SportsStats/api/Sports").then(function (success) {
                            $scope.data = success.data;
                            $rootScope.ShowSpinner = false;
                        }, function (error) {
                        });
                    }
                }
            });

            $scope.GoToTeam = function (selection) {
                $scope.UpdateLeagueSelection(selection);
                $location.url("/SportsStats/Teams");
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

            //$scope.UpdateSportSelection = function () {
            //    $rootScope.ShowSpinner = true;
            //    $scope.GetLeagues($scope.SelectedSportID);
            //};
        }
    ]);

}());