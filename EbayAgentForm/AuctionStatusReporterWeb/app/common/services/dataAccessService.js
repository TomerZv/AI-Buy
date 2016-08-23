var app;
(function (app) {
    var common;
    (function (common) {
        var DataAccessService = (function () {
            function DataAccessService($resource) {
                this.$resource = $resource;
            }
            DataAccessService.prototype.getAuctionResource = function () {
                return this.$resource("http://localhost:8670/api/ManageAuctions/GetAuctions");
            };
            DataAccessService.prototype.getSingleAuctionResource = function (id) {
                return this.$resource("http://localhost:8670/api/ManageAuctions/GetAuction?id=" + id);
            };
            DataAccessService.$inject = ["$resource"];
            return DataAccessService;
        }());
        common.DataAccessService = DataAccessService;
        angular
            .module("common.services")
            .service("dataAccessService", DataAccessService);
    })(common = app.common || (app.common = {}));
})(app || (app = {}));
//# sourceMappingURL=dataAccessService.js.map