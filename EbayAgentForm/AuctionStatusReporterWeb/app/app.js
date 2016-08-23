var app;
(function (app) {
    angular.module("auctionReporter", ["ngRoute", "common.services"]);
    angular.module("auctionReporter").config(routeConfig);
    routeConfig.$inject = ["$routeProvider"];
    function routeConfig($routeProvider) {
        $routeProvider
            .when("/", {
            templateUrl: "/app/auctions/auctionReporterView.html",
            controller: "AuctionReporterCtrl",
            controllerAs: "vm"
        })
            .otherwise("/");
    }
})(app || (app = {}));
//# sourceMappingURL=app.js.map