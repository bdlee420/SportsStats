/*jshint sub:true*/

(function () {
    "use strict";

    sportsApp.controller('gamesController', ['$scope', '$http', '$location', '$rootScope', 'CurrentStateFactory',
        function gamesController($scope, $http, $location, $rootScope, CurrentStateFactory) {
            $rootScope.RedirectURL = "Games";
            var currentState;
            $rootScope.ShowSpinner = true;
            CurrentStateFactory.getUser().then(function (user) {
                if (user.data == null) {
                    $rootScope.ShowSpinner = false;
                    $location.url("/SportsStats/Login?redirect=Games");
                }
                else {
                    currentState = CurrentStateFactory.getState(true, user.data.UserName);
                    if (!currentState) {
                        return;
                    }
                    $scope.ReadOnly = user.data.RoleID !== 1;
                    $http.get("/SportsStats/api/Games/GetGames?leagueID=" + currentState.SelectedLeagueID).then(function (success) {
                        var data = success.data;
                        $scope.ListGames = data.Games;
                        $scope.ListTeams = data.Teams;
                        $rootScope.ShowSpinner = false;
                    }, function (error) {
                    });
                }
            });

            $scope.AddGame = function (game) {
                var newGame = jQuery.extend(true, {}, game);
                newGame.Team1ID = parseInt(newGame.Team1ID);
                newGame.Team2ID = parseInt(newGame.Team2ID);
                newGame.LeagueID = currentState.SelectedLeagueID;
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Games/AddGame', newGame).then(function (success) {
                    var data = success.data;
                    $scope.ListGames = data.Games;
                    $scope.ListTeams = data.Teams;
                    game.GameDate = null;
                    game.Team1ID = null;
                    game.Team2ID = null;
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.GoToGame = function (game) {
                $location.url("/SportsStats/Games/" + game.ID);
            };
        }
    ]);

    sportsApp.controller('gameController', ['$scope', '$http', '$routeParams', '$location', '$rootScope', 'CurrentStateFactory', '$interval',
        function gameController($scope, $http, $routeParams, $location, $rootScope, CurrentStateFactory, $interval) {
            $rootScope.RedirectURL = "Games/" + $routeParams.gameID;
            var currentState;
            $scope.touched = false;
            $scope.touching = false;
            $scope.moved = false;
            $scope.moving = false;
            $scope.lastMove = null;

            $rootScope.ShowSpinner = true;

            $scope.LoadData = function (partial) {
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
                        if (!partial) {
                            if (currentState.SelectedSportID === 2) {
                                $scope.IsBaseball = true;
                                $scope.ShowOrder = false;
                                $scope.ShowAll = false;
                            }
                            else {
                                $scope.IsBaseball = false;
                                $scope.ShowOrder = false;
                                $scope.ShowAll = true;
                            }
                        }

                        $scope.ReadOnly = user.data.RoleID !== 1;
                        $http.get("/SportsStats/api/Games/GetGame?leagueID=" + currentState.SelectedLeagueID + "&gameID=" + $routeParams.gameID).then(function (success) {
                            var data = success.data;
                            $scope.LoadGridData(data);
                            $rootScope.ShowSpinner = false;
                            $scope.IsGamePage = true;
                            if ($scope.IsBaseball) {
                                $scope.BaseballGameStateResult = data.BaseballGameStateResult;

                                $scope.InningText = $scope.GetInningText(data.BaseballGameStateResult);
                            }
                        }, function (error) {
                        });
                    }
                });
            };

            $scope.LoadData(false);

            $scope.SetAutoRefresh = function () {
                $scope.AutoRefresh = !$scope.AutoRefresh;
                if ($scope.AutoRefresh === true) {
                    $scope.Timer = $interval(function () {
                        $scope.LoadData(true);
                    }, 20000);
                }
                else {
                    if (angular.isDefined($scope.Timer)) {
                        $interval.cancel($scope.Timer);
                    }
                }
            };

            $scope.HideShowButtons = function () {
                $scope.HideButtons = !$scope.HideButtons;                
            };            

            $scope.GetInningText = function (state) {
                return (state.TopOfInning ? "Top" : "Bottom") + " " + state.Inning;
            };

            $scope.GoToStats = function (player) {
                if (!$scope.ReadOnly) {
                    $location.url("/SportsStats/Stats/" + player.GameID + "/" + player.TeamID + "/" + player.PlayerID);
                }
            };

            $scope.ShowMessage = function (player, statText, teamNumber) {
                var message = player.PlayerName + ": " + statText;
                if (teamNumber === 1) {
                    $rootScope.LastStatTeam1 = message;
                }
                else {
                    $rootScope.LastStatTeam2 = message;
                }
            };

            $scope.AddStat = function (player, statID, teamNumber, statText) {    
                var newStat = {
                	PlayerID: player.PlayerID,
					LeagueID: currentState.SelectedLeagueID,
                    GameID: player.GameID,
                    TeamID: player.TeamID,
                    StatTypeID: statID,
                    Value: 1,
                    IsBaseball: $scope.IsBaseball
                };
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Stats/AddStat', newStat).then(function (success) {
                    $scope.LoadData(true);
                    $rootScope.ShowSpinner = false;
                    $scope.ShowMessage(player, statText, teamNumber);
                }, function (error) {
                });
            };

            $scope.SetActive = function (player, isActive, teamNumber) {               

                var newStat = {
                    PlayerID: player.PlayerID,
                    LeagueID: currentState.SelectedLeagueID,
                    GameID: player.GameID,
                    TeamID: player.TeamID,
                    StatTypeID: 200,
                    Value: isActive ? 1 : -1,
                    IsBaseball: $scope.IsBaseball
                };
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Stats/AddStat', newStat).then(function (success) {
                    $scope.LoadData(true);
                    $rootScope.ShowSpinner = false;
                    var statText = isActive ? "Into the game" : "Out of the game";
                    $scope.ShowMessage(player, statText, teamNumber);
                }, function (error) {
                });
            };

            $scope.SaveOrder = function (player, teamID, reload) {
                $rootScope.ShowSpinner = true;
                var playerStat = {
                    StatTypeID: 72, //batting order
                    PlayerID: player.PlayerID,
                    GameID: $routeParams.gameID,
                    TeamID: teamID,
                    Value: player.Order,
                };
                $http.post('/SportsStats/api/Stats/SaveOrder', playerStat).then(function (success) {
                    if (reload) {
                        $scope.LoadData(true);
                    }
                    else {
                        $rootScope.ShowSpinner = false;
                    }

                }, function (error) {
                });
            };

            $scope.GetQuickButtonClass = function (isPositive) {
                return isPositive ? "quickcell positivecell" : "quickcell negativecell";
            };

            $scope.GetQuickButtonClass2 = function (isPositive) {
                return isPositive ? "quickcell2 positivecell" : "quickcell2 negativecell";
            };

            $scope.GetActivePlayerClass = function (isActivePlayer, showOrder) {
                var className = "firstcolumn ";
                if (isActivePlayer) {
                    className += " activePlayer ";
                }
                if (showOrder) {
                    className += " headcol2 ";
                }
                else {
                    className += " headcol2b ";
                }
                return className;

            };
            //ng - class="Player.IsActivePlayer ? 'activePlayer' : ''" ng - click="GoToStats(Player)" class="headcol2 firstcolumn" ng - class="ShowOrder ? 'headcol2' : 'headcol2b'"

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

            $scope.SaveGame = function (game) {
                game.LeagueID = currentState.SelectedLeagueID;
                game.Team1ID = parseInt(game.Team1ID);
                game.Team2ID = parseInt(game.Team2ID);
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Games/UpdateGame', game).then(function (success) {
                    var data = success.data;
                    $scope.LoadGridData(data);
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.LoadGridData = function (data) {
                var gameDate = new Date(data.GameDate);
                var convertedGameDate = new Date(gameDate.getTime() + (gameDate.getTimezoneOffset() * 60000));
                data.GameDate = convertedGameDate;
                $scope.Game = data;
                $scope.Game.GameDate = new Date($scope.Game.GameDate);
                $scope.Game.Team1ID = parseInt($scope.Game.Team1ID);
                $scope.Game.Team2ID = parseInt($scope.Game.Team2ID);
                $scope.ListTeams = data.Teams;

                new UpdateStatsObject(data.StatTypes, data.Team1PlayerStats, data.Team1ID, data.ID);

                new UpdateStatsObject(data.StatTypes, data.Team2PlayerStats, data.Team2ID, data.ID);

                new UpdateStatsObject(data.StatTypes, data.TotalTeam1Stats, data.Team1ID, data.ID);

                new UpdateStatsObject(data.StatTypes, data.TotalTeam2Stats, data.Team2ID, data.ID);
            };

            $scope.GoToLog = function () {
                $location.url("/SportsStats/GameLog/" + $scope.Game.ID);
            };

            $scope.AddPlayer = function (player, teamID) {
                var newPlayer = jQuery.extend(true, {}, player);
                if (!newPlayer.Number || newPlayer.Number === 0) {
                	newPlayer.Number = player.Number2;
                }
                newPlayer.LeagueID = currentState.SelectedLeagueID;
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Games/AddPlayer', { Player: newPlayer, TeamID: teamID }).then(function (success) {
                    var playerID = success.data;
                    $scope.SaveOrder({ PlayerID: playerID, Order: player.Order }, teamID, true);
                    player.Name = null;
                    player.Number = null;
                    player.Number2 = null;
                    player.Order = null;
                    player.ID = null;
                }, function (error) {
                });
            };

        }
    ]);

}());