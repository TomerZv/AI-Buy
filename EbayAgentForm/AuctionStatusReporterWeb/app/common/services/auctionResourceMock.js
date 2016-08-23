//module app.common {
//    var mockResource = angular.module("auctionResourceMock", ["ngMockE2E"]);
//    mockResource.run(mockRun);
//    mockRun.$inject = ["$httpBackend"];
//    function mockRun($httpBackend): void {
//        var auctions = new Array<app.businessObjects.IAuction>(0);
//        var auction;
//        auction = new app.businessObjects.Auction(1, "iPhone", new Date(), 1000, new Date(), 10, 2000, 10, new Array<app.businessObjects.IBid>(0));
//        this.auctions.push(auction);
//        auction = new app.businessObjects.Auction(2, "LG", new Date(), 1000, new Date(), 10, 2000, 10, new Array<app.businessObjects.IBid>(0));
//        this.auctions.push(auction);
//        var auctionUrl = "/api/auctions";
//        $httpBackend.whenGET(auctionUrl).respond(auctions);
//    }
//} 
//# sourceMappingURL=auctionResourceMock.js.map