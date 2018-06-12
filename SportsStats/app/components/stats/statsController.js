(function () {
    "use strict";

    sportsApp.controller('statsController', ['$scope', '$http',
        function statsController($scope, $http) {

        }
    ]);

    sportsApp.controller('statController', ['$scope', '$http', '$routeParams', '$location', '$rootScope', 'CurrentStateFactory', '$route',
        function statController($scope, $http, $routeParams, $location, $rootScope, CurrentStateFactory, $route) {
            $rootScope.RedirectURL = "Stats/" + $routeParams.gameID + "/" + $routeParams.teamID + "/" + $routeParams.playerID;
            $rootScope.ShowSpinner = true;
            var currentState;

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
                    if (currentState.SelectedSportID === 2) {
                        $scope.IsBaseball = true;
                        $scope.StayOnPage = true;
                    }
                    $scope.DataSaved = true;
                    $scope.LoadData();
                }
            });

            $scope.LoadData = function () {
                $http.get("/SportsStats/api/Stats/GetStats?sportID=" + currentState.SelectedSportID + "&playerID=" + $routeParams.playerID + "&gameID=" + $routeParams.gameID + "&teamID=" + $routeParams.teamID + "&groupID=null&leagueID=" + currentState.SelectedLeagueID).then(function (success) {
                    var data = success.data;
                    if ($rootScope.AutoBatter) {
                        $('#myModalNewBatter').modal('show');
                        $rootScope.AutoBatter = false;
                    }

                    $scope.IsGamePage = false;
                    $scope.OperatorText = "+";
                    $scope.Stats = data;
                    $scope.Stats.LatestGroupID = data.GroupID;
                    if ($scope.IsBaseball) {
                        $scope.BaseballGameStateResult = data.BaseballGameStateResult;
                        $scope.InningText = $scope.GetInningText(data.BaseballGameStateResult);
                    }
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });

            };

            $scope.LoadPartialData = function (groupID) {
                $http.get("/SportsStats/api/Stats?sportID=" + currentState.SelectedSportID + "&playerID=" + $routeParams.playerID + "&gameID=" + $routeParams.gameID + "&teamID=" + $routeParams.teamID + "&groupID=" + groupID + "&leagueID=" + currentState.SelectedLeagueID).then(function (success) {
                    var data = success.data;
                    $scope.Stats.StatTypes = data.StatTypes;
                    $scope.Stats.StatGroups = data.StatGroups;
                    $scope.Stats.GroupID = data.GroupID;
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.RefreshPage = function () {
                $('#myModal').modal('hide');
                $scope.LoadData();
            };

            $scope.NewGroup = function () {
                var newStatGroup = {
                    PlayerID: $routeParams.playerID,
                    GameID: $routeParams.gameID,
                    TeamID: $routeParams.teamID
                };

                $http.post('/SportsStats/api/Stats/NewGroup', newStatGroup).then(function (success) {
                    if (!$scope.StayOnPage) {
                        $rootScope.ShowSpinner = false;
                        $scope.CloseStats();
                    }
                    else {
                        $scope.LoadPartialData(null);
                    }
                }, function (error) {
                });
            };

            $scope.GoToGroup = function (groupID) {
                $scope.LoadPartialData(groupID);
            };

            $scope.ChangeOperator = function () {
                $scope.OperatorText = $scope.OperatorText === "-" ? "+" : "-";
                $scope.IsNegative = $scope.OperatorText === "-";
            };

            $scope.ShowGameState = function () {
                $('#myModal').modal('show');
            };

            $scope.CloseStats = function () {
                $location.url("/SportsStats/Games/" + $routeParams.gameID);
            };

            $scope.ChangeOuts = function (val) {
                $scope.BaseballGameStateResult.NumberOfOuts = $scope.BaseballGameStateResult.NumberOfOuts + val;
                if ($scope.BaseballGameStateResult.NumberOfOuts < 0) {
                    $scope.BaseballGameStateResult.NumberOfOuts = 0;
                }
                else if ($scope.BaseballGameStateResult.NumberOfOuts > 3) {
                    $scope.BaseballGameStateResult.NumberOfOuts = 3;
                }
                $scope.InningChanged = $scope.BaseballGameStateResult.NumberOfOuts === 3;
            };

            $scope.MoveRunner = function (player, oldBase, newBase) {
                if (player != null) {
                    var success = false;
                    if (newBase === 0) {
                        if ($scope.BaseballGameStateResult.RunnersOut == null) {
                            $scope.BaseballGameStateResult.RunnersOut = [];
                        }
                        $scope.BaseballGameStateResult.RunnersOut.push(player);
                        $scope.BaseballGameStateResult.NumberOfOuts++;
                        success = true;
                    }
                    else if (newBase === 1 && $scope.BaseballGameStateResult.PlayerOnFirst == null) {
                        $scope.BaseballGameStateResult.PlayerOnFirst = player;
                        success = true;
                    }
                    else if (newBase === 2 && $scope.BaseballGameStateResult.PlayerOnSecond == null) {
                        $scope.BaseballGameStateResult.PlayerOnSecond = player;
                        success = true;
                    }
                    else if (newBase === 3 && $scope.BaseballGameStateResult.PlayerOnThird == null) {
                        $scope.BaseballGameStateResult.PlayerOnThird = player;
                        success = true;
                    }
                    else if (newBase === 4) {
                        if ($scope.BaseballGameStateResult.RunnersScored == null) {
                            $scope.BaseballGameStateResult.RunnersScored = [];
                        }
                        $scope.BaseballGameStateResult.RunnersScored.push(player);
                        success = true;
                    }

                    if (success) {
                        if (oldBase === 0) {
                            var arrayOuts = $scope.BaseballGameStateResult.RunnersOut.filter(function (obj) {
                                return obj.ID !== player.ID;
                            });
                            $scope.BaseballGameStateResult.RunnersOut = arrayOuts;
                            $scope.BaseballGameStateResult.NumberOfOuts--;
                        }
                        else if (oldBase === 1) {
                            $scope.BaseballGameStateResult.PlayerOnFirst = null;
                        }
                        else if (oldBase === 2) {
                            $scope.BaseballGameStateResult.PlayerOnSecond = null;
                        }
                        else if (oldBase === 3) {
                            $scope.BaseballGameStateResult.PlayerOnThird = null;
                        }
                        else if (oldBase === 4) {
                            var arrayScored = $scope.BaseballGameStateResult.RunnersScored.filter(function (obj) {
                                return obj.ID !== player.ID;
                            });
                            $scope.BaseballGameStateResult.RunnersScored = arrayScored;
                        }
                    }
                }
            };

            $scope.GetGroupClass = function (groupID) {
                return groupID === $scope.Stats.GroupID ? "greenbackground" : "";
            };

            $scope.SaveNewGameState = function () {
                $rootScope.ShowSpinner = true;
                $scope.BaseballGameStateResult.LeagueID = currentState.CurrentLeagueID;
                $http.post('/SportsStats/api/Stats/SaveNewGameState', $scope.BaseballGameStateResult).then(function (success) {
                    var data = success.data;
                    if (data.PlayerID != null) {
                        $rootScope.AutoBatter = true;
                        $location.url("/SportsStats/Stats/" + $routeParams.gameID + "/" + data.TeamID + "/" + data.PlayerID);
                    }
                    else {
                        $location.url("/SportsStats/Games/" + $routeParams.gameID);
                    }
                    $rootScope.ShowSpinner = false;
                }, function (error) {
                });
            };

            $scope.GetInningText = function (state) {
                return (state.TopOfInning ? "Top" : "Bottom") + " " + state.Inning;
            };

            $scope.AddStat = function (statType, statStates) {
                var newStat = jQuery.extend(statType,
                    {
                        PlayerID: $routeParams.playerID,
                        GameID: $routeParams.gameID,
                        TeamID: $routeParams.teamID,
                        GroupID: $scope.Stats.GroupID,
                        StatTypeID: statType.ID,
                        Value: $scope.IsNegative ? -1 : 1,
                        StatStates: statStates,
                        IsLatestGroup: $scope.Stats.LatestGroupID === $scope.Stats.GroupID
                    });
                $rootScope.ShowSpinner = true;
                $http.post('/SportsStats/api/Stats/AddStat', newStat).then(function (success) {
                    var data = success.data;
                    $rootScope.ShowSpinner = false;

                    if ($scope.IsBaseball) {
                        $scope.DataSaved = true;
                        $scope.BaseballGameStateResult = data;
                        $scope.InningText = $scope.GetInningText(data);
                        $scope.DataSaved = data.DataSaved;
                        $scope.InningChanged = data.InningChanged;

                        if ($scope.DataSaved) {
                            $scope.BaseballGameStateResult.GameStat = null;
                            if (data.NextAtBat && data.NextBatter != null) {
                                if (data.NextBatter.PlayerID != null) {
                                    $rootScope.AutoBatter = true;
                                    if (data.NextBatter.PlayerID === parseInt($routeParams.playerID)) {
                                        $scope.RefreshPage();
                                    }
                                    else {
                                        $location.url("/SportsStats/Stats/" + $routeParams.gameID + "/" + data.NextBatter.TeamID + "/" + data.NextBatter.PlayerID);
                                    }
                                }
                                else {
                                    $location.url("/SportsStats/Games/" + $routeParams.gameID);
                                }
                            }
                            else {
                                $scope.LoadPartialData($scope.Stats.GroupID);
                            }
                        }
                        else {
                            $('#myModal').modal('show');
                        }

                        if (!$scope.StayOnPage) {
                            $rootScope.ShowSpinner = false;
                            $scope.CloseStats();
                        }
                    }
                    else {
                        if ($scope.StayOnPage) {
                            $scope.LoadPartialData($scope.Stats.GroupID);
                        }
                        else {
                            $location.url("/SportsStats/Games/" + $routeParams.gameID);
                        }
                    }    
                }, function (error) {
                });
            };
        }
    ]);

}());