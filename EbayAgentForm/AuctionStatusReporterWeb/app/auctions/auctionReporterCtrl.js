var app;
(function (app) {
    var auctions;
    (function (auctions) {
        var AuctionReporterCtrl = (function () {
            function AuctionReporterCtrl(dataAccessService) {
                var _this = this;
                this.dataAccessService = dataAccessService;
                this.auctions = new Array(0);
                var auctionResource = this.dataAccessService.getAuctionResource();
                auctionResource.query(function (data) {
                    data.forEach(function (auction) { return _this.auctions.push(new app.businessObjects.Auction().deserialize(auction)); });
                });
            }
            AuctionReporterCtrl.$inject = ["dataAccessService"];
            return AuctionReporterCtrl;
        }());
        angular.module("auctionReporter")
            .controller("AuctionReporterCtrl", AuctionReporterCtrl);
    })(auctions = app.auctions || (app.auctions = {}));
})(app || (app = {}));
//# sourceMappingURL=auctionReporterCtrl.js.map