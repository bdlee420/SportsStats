(function () {
    "use strict";

    sportsApp.controller('teamsController', ['$scope', '$http', '$routeParams', '$rootScope', '$location', 'CurrentStateFactory',
        function teamsController($scope, $http, $routeParams, $rootScope, $location, CurrentStateFactory) {
            $rootScope.RedirectURL = "Teams";
            var currentState;
            $rootScope.ShowSpinner = true;

            CurrentStateFactory.getUser().then(function (user) {
                if (user.data == null || $routeParams.user != null) {
                    $rootScope.ShowSpinner = false;
                    $location.url("/SportsStats/Login?user=" + $routeParams.user);
                }
                else {
                    currentState = CurrentStateFactory.getState(true, user.data.UserName);
                    if (!currentState) {
                        return;
                    }
                    $scope.ReadOnly = user.data.RoleID !== 1 && !user.data.AdminLeagueIDs.includes(currentState.SelectedLeagueID);
                    $http.get("/SportsStats/api/Teams/GetTeams?leagueID=" + currentState.SelectedLeagueID + "&sportID=" + currentState.SelectedSportID).then(function (success) {
                        var data = success.data;
                        $rootScope.ShowSpinner = false;
                        if ($rootScope.HasOneTeam) {
                            var teamID = data.Teams[0].ID;
                            $location.url("/SportsStats/Teams/" + teamID);
                        }
                        else {
                            $scope.bindData(data);
                        }
                    }, function (error) {
                    });
                }
            });

            $scope.AddTeam = function (team) {
                var newTeam = jQuery.extend(true, {}, team);
                newTeam.LeagueID = currentState.SelectedLeagueID;
                newTeam.SportID = currentState.SelectedSportID;
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Teams/AddTeam', newTeam).then(function (success) {
                    var data = success.data;
                    $scope.bindData(data);
                    team.Name = null;
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.bindData = function (data) {
                $scope.ListTeams = data.Teams;
                $scope.ListAvailableTeams = data.AvailableTeams;
            };
        }
    ]);

    sportsApp.controller('teamController', ['$scope', '$http', '$routeParams', '$location', '$rootScope', 'CurrentStateFactory',
        function teamController($scope, $http, $routeParams, $location, $rootScope, CurrentStateFactory) {
            $rootScope.RedirectURL = "Teams/" + $routeParams.teamID;
            var currentState;
            $rootScope.ShowSpinner = true;
            $scope.touched = false;
            $scope.touching = false;
            $scope.moved = false;
            $scope.moving = false;
            $scope.lastMove = null;
            $scope.baseballCurrentSort = "AVG";
            $scope.hockeyCurrentSort = "Points";
            $scope.basketballCurrentSort = "Points Per Game";
            $scope.currentSort = "";
            $scope.currentSortAsc = true;

            $scope.LoadData = function () {
                CurrentStateFactory.getUser().then(function (user) {
                    if (user.data == null || $routeParams.user != null) {
                        $rootScope.ShowSpinner = false;
                        $location.url("/SportsStats/Login?user=" + $routeParams.user);
                    }
                    else {
                        currentState = CurrentStateFactory.getState(true, user.data.UserName);
                        if (!currentState) {
                            return;
                        }
                        if (currentState.SelectedSportID === 1) {
                            $scope.currentSort = $scope.basketballCurrentSort;
                        }
                        else if (currentState.SelectedSportID === 2) {
                            $scope.currentSort = $scope.baseballCurrentSort;
                        }
                        else if (currentState.SelectedSportID === 3) {
                            $scope.currentSort = $scope.hockeyCurrentSort;
                        }

                        $scope.ReadOnly = user.data.RoleID !== 1 && !user.data.AdminLeagueIDs.includes(currentState.SelectedLeagueID);
                        $http.get("/SportsStats/api/Teams/GetTeam?teamID=" + $routeParams.teamID + "&leagueID=" + currentState.SelectedLeagueID).then(function (success) {
                            var data = success.data;
                            $scope.Team = data;
                            $scope.ListTeams = data.Teams;
                            $scope.ListAvailablePlayers = data.AvailablePlayers;

                            for (var i = 0; i < data.Games.length; i++) {
                                var game = data.Games[i];
                                if (game.HighScore === 0 && game.LowScore === 0) {
                                    jQuery.extend(game, { GameResult: 0, GameResultText: "" });
                                }
                                else {
                                    if (game.DidWin) {
                                        jQuery.extend(game, { GameResult: 1, GameResultText: "WON" });
                                    }
                                    else if (game.DidWin == null) {
                                        jQuery.extend(game, { GameResult: 3, GameResultText: "TIE" });
                                    }
                                    else if (!game.DidWin) {
                                        jQuery.extend(game, { GameResult: 2, GameResultText: "LOSS" });
                                    }
                                    else {
                                        jQuery.extend(game, { GameResult: 0, GameResultText: "" });
                                    }
                                }
                            }
                            console.log(data.TeamPlayerStats);
                            console.log(data.TeamTotalStats);
                            new UpdateStatsObject(data.StatTypes, data.TeamPlayerStats, data.Team1ID, data.ID);
                            new UpdateStatsObject(data.StatTypes, data.TeamTotalStats, data.Team1ID, data.ID);

                            $scope.SortStats($scope.currentSort);

                            $rootScope.ShowSpinner = false;
                        }, function (error) {
                        });
                    }
                });
            };

            $scope.SortStats = function (fieldName) {
                if ($scope.currentSort === fieldName) {
                    $scope.currentSortAsc = !$scope.currentSortAsc;
                }
                $scope.currentSort = fieldName;
                $scope.Team.TeamPlayerStats.sort($scope.Sort);
            };

            $scope.Sort = function (a, b) {
                var asc = $scope.currentSortAsc;
                var object1 = a[$scope.currentSort];
                var object2 = b[$scope.currentSort];
                var isNumeric = !isNaN(parseFloat(object1)) && isFinite(object2);
                if (isNumeric) {
                    if (object1 === object2) { return 0; }
                    if (object1 > object2) {
                        return (asc ? 1 : -1);
                    }
                    else {
                        return (asc ? -1 : 1);
                    }
                }
                else {
                    var aName = object1.toLowerCase();
                    var bName = object2.toLowerCase();
                    return ((aName < bName) ? -1 : ((aName > bName) ? (asc ? 1 : 0) : (asc ? 0 : 1)));
                }
            };

            $scope.LoadData();

            $scope.start = function () {
                $scope.touched = true;
                $scope.touching = true;
            };

            $scope.move = function () {
                $scope.moving = true;
                $scope.moved = true;
                $scope.lastMove = Date.now();
            };

            $scope.end = function () {
                $scope.moving = false;
                $scope.touching = false;
            };

            $scope.AddPlayer = function (player) {
                var newPlayer = jQuery.extend(true, {}, player);
                if (!newPlayer.Number || newPlayer.Number === 0) {
                    newPlayer.Number = player.Number2;
                }
                newPlayer.LeagueID = currentState.SelectedLeagueID;
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Teams/AddPlayer', { Player: newPlayer, TeamID: $scope.Team.ID, LeagueID: currentState.SelectedLeagueID }).then(function (success) {
                    $scope.LoadData();
                    player.Name = null;
                    player.Number = null;
                    player.Number2 = null;
                    player.ID = null;
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.SaveTeam = function (team) {
                if (!team || !team.ID) { return; }
                 var payload = {
                     ID: team.ID,
                     Name: team.Name,
                     LeagueID: currentState.SelectedLeagueID,
                     SportID: currentState.SelectedSportID
                 };
                 $rootScope.ShowSpinner = true;
                 $http.post('/SportsStats/api/Teams/UpdateTeam', payload).then(function (success) {
                     var data = success.data;
                     // Replace team data on page with refreshed result
                     if (data) {
                         $scope.Team = data;
                     }
                     $rootScope.ShowSpinner = false;
                 }, function (error) {
                     $rootScope.ShowSpinner = false;
                 });
            };

            $scope.GoToGame = function (id) {
                $location.url("/SportsStats/Games/" + id);
            };

            $scope.GoToPlayer = function (id) {
                $location.url("/SportsStats/Players/" + id);
            };

            $scope.AddGame = function (game, team) {
                var newGame = jQuery.extend(true, {}, game);
                newGame.Team1ID = parseInt(team.ID);
                newGame.Team2ID = parseInt(newGame.Team2ID);
                newGame.LeagueID = currentState.SelectedLeagueID;
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Teams/AddGame', newGame).then(function (success) {
                    var data = success.data;
                    $scope.Team = data;
                    $scope.ListTeams = data.Teams;
                    $scope.ListAvailablePlayers = data.AvailablePlayers;

                    for (var i = 0; i < data.Games.length; i++) {
                        var game = data.Games[i];
                        if (game.HighScore === 0 && game.LowScore === 0) {
                            jQuery.extend(game, { GameResult: 0, GameResultText: "" });
                        }
                        else {
                            if (game.DidWin) {
                                jQuery.extend(game, { GameResult: 1, GameResultText: "WON" });
                            }
                            else if (game.DidWin == null) {
                                jQuery.extend(game, { GameResult: 3, GameResultText: "TIE" });
                            }
                            else if (!game.DidWin) {
                                jQuery.extend(game, { GameResult: 2, GameResultText: "LOSS" });
                            }
                            else {
                                jQuery.extend(game, { GameResult: 0, GameResultText: "" });
                            }
                        }
                    }

                    new UpdateStatsObject(data.StatTypes, data.TeamPlayerStats, data.Team1ID, data.ID);

                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.DeleteGame = function (game, $event) {
                $event.stopPropagation();
                if (!confirm('Delete this game?')) {
                    return;
                }
                $rootScope.ShowSpinner = true;
                // send the game ID and leagueID so server can authorize and delete
                $http.post('/SportsStats/api/Games/DeleteGame', { ID: game.ID, LeagueID: currentState.SelectedLeagueID }).then(function (success) {
                    // Reload team info from server to refresh game list and stats
                    $scope.LoadData();
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                    if (error.status === 403) {
                        alert('You are not authorized to delete games.');
                    }
                    $rootScope.ShowSpinner = false;
                });
            };

            $scope.GetResultClass = function (didWin) {
                if (didWin) {
                    return "messagegreen";
                }
                else if (didWin == null) {
                    return "messageblue";
                }
                else if (!didWin) {
                    return "messagered";
                }
                return "";
            };
        }
    ]);

}());