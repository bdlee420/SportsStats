(function () {
    "use strict";

    sportsApp.controller('leaguesController', ['$scope', '$routeParams', '$rootScope', '$http', '$cookies', '$location', 'CurrentStateFactory',
        function leaguesController($scope, $routeParams, $rootScope, $http, $cookies, $location, CurrentStateFactory) {
            var currentState;
            $rootScope.ShowSpinner = true;
            CurrentStateFactory.getUser().then(function (user) {
                if (user.data == null || $routeParams.user != null) {
                    $rootScope.ShowSpinner = false;
                    $location.url("/SportsStats/Login?user=" + $routeParams.user);
                }
                else {
                    $scope.UserName = user.data.UserName;
                    $scope.IsAdmin = user.data.RoleID === 1;
                    if (user.data.HasOneLeague && user.data.HasOneTeam) {
                        var sportID = user.data.Sports[0];
                        var leagueID = user.data.Leagues[0];
                        $scope.UpdateLeagueSelection({ SportID: sportID, LeagueID: leagueID });
                        if ($rootScope.RedirectURL != null) {
                            $location.url("/SportsStats/" + $rootScope.RedirectURL);
                        }
                        else {
                            $location.url("/SportsStats/League");
                        }
                    }
                    else {
                        currentState = CurrentStateFactory.getState(false, $scope.UserName);
                        $scope.SelectedSportID = currentState.SelectedSportID;
                        $scope.SelectedLeagueID = currentState.SelectedLeagueID;
                        $http.get("/SportsStats/api/Leagues/GetLeagues").then(function (success) {
                            $scope.data = success.data;
                            $rootScope.ShowSpinner = false;
                        }, function (error) {
                        });

                        // Load sports for add league form
                        $http.get("/SportsStats/api/Sports/GetSports").then(function (success) {
                            $scope.Sports = success.data;
                        }, function (error) {
                        });

                        // Load seasons for add league form
                        $http.get("/SportsStats/api/Leagues/GetSeasons").then(function (success) {
                            $scope.Seasons = success.data;
                        }, function (error) {
                        });

                        // initialize new league object
                        $scope.NewLeague = { SportID: null, Name: "", StartDate: null, EndDate: null, SeasonID: 1 };

                        $scope.SaveLeague = function () {
                            if (!$scope.IsAdmin) {
                                return;
                            }
                            // SeasonID is hardcoded in server code as 4 seasons, but we'll pass what user selected if any (default 1)
                            var payload = {
                                SportID: $scope.NewLeague.SportID,
                                Name: $scope.NewLeague.Name,
                                StartDate: $scope.NewLeague.StartDate,
                                EndDate: $scope.NewLeague.EndDate,
                                SeasonID: $scope.NewLeague.SeasonID
                            };
                            $rootScope.ShowSpinner = true;
                            $http.post('/SportsStats/api/Leagues/AddLeague', payload).then(function (success) {
                                // refresh list
                                $http.get("/SportsStats/api/Leagues/GetLeagues").then(function (success) {
                                    $scope.data = success.data;
                                    $rootScope.ShowSpinner = false;
                                    // collapse form
                                    $('#addLeagueForm').collapse('hide');
                                }, function (error) {
                                });
                            }, function (error) {
                                $rootScope.ShowSpinner = false;
                            });
                        };
                    }
                }
            });

            $scope.GoToLeague = function (selection) {
                $scope.UpdateLeagueSelection(selection);
                $location.url("/SportsStats/League");
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
        }
    ]);

}());