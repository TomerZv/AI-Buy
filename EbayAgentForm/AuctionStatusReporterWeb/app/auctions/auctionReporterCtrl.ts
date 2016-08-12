module app.auctions {
    interface IAuctionReporterCtrl {
        auctions: app.businessObjects.IAuction[];
    }

    class AuctionReporterCtrl implements IAuctionReporterCtrl {
        public auctions: app.businessObjects.IAuction[]

        static $inject = ["dataAccessService"];
        constructor(private dataAccessService: app.common.DataAccessService) {
            this.auctions = new Array<app.businessObjects.IAuction>(0);

            var auctionResource = this.dataAccessService.getAuctionResource();

            auctionResource.query((data: app.businessObjects.IAuction[]) => {
                data.forEach(auction => this.auctions.push(new app.businessObjects.Auction().deserialize(auction)));
            });
        }
    }

    angular.module("auctionReporter")
        .controller("AuctionReporterCtrl", AuctionReporterCtrl);
} 