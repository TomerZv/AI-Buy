module app {
    angular.module("auctionReporter", ["ngRoute", "common.services"]);
    angular.module("auctionReporter").config(routeConfig);

    routeConfig.$inject = ["$routeProvider"];
    function routeConfig($routeProvider : ng.route.IRouteProvider) : void {
        $routeProvider
            .when("/",
            {
                templateUrl: "/app/auctions/auctionReporterView.html",
                controller: "AuctionReporterCtrl",
                controllerAs: "vm"
            })
            .otherwise("/");
    }
} 