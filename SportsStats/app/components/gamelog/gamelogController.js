(function () {
    "use strict";    

    sportsApp.controller('gamelogController', ['$scope', '$http', '$routeParams', '$rootScope', '$location', 'CurrentStateFactory',
        function gamelogController($scope, $http, $routeParams, $rootScope, $location, CurrentStateFactory) {
            $rootScope.RedirectURL = "GameLog/" + $routeParams.gameID;
            $rootScope.ShowSpinner = true;
            CurrentStateFactory.getUser().then(function (user) {
                if (user.data == null) {
                    $rootScope.ShowSpinner = false;             
                    $location.url("/SportsStats/Login");
                }
                else {
                    $scope.ReadOnly = user.data.RoleID !== 1;
                    $http.get("/SportsStats/api/Games/GetGameLog?gameID=" + $routeParams.gameID).then(function (success) {
                        var data = success.data;
                        $scope.Logs = data;
                        $rootScope.ShowSpinner = false;
                    }, function (error) {
                    });
                }
            });          
        }
    ]);

}());