(function () {
    "use strict";

    sportsApp.controller('leagueController', ['$scope', '$http', '$routeParams', '$rootScope', '$location', 'CurrentStateFactory',
        function leagueController($scope, $http, $routeParams, $rootScope, $location, CurrentStateFactory) {
            $rootScope.RedirectURL = "League";
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
                    $http.get("/SportsStats/api/League/GetTeams?leagueID=" + currentState.SelectedLeagueID + "&sportID=" + currentState.SelectedSportID).then(function (success) {
                        var data = success.data;
                        $rootScope.ShowSpinner = false;
                        if ($rootScope.HasOneTeam) {
                            var teamID = data.Teams[0].ID;
                            $location.url("/SportsStats/Team/" + teamID);
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
                $http.post('/SportsStats/api/League/AddTeam', newTeam).then(function (success) {
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

}());