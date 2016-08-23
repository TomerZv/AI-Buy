var app;
(function (app) {
    var auctions;
    (function (auctions) {
        var AuctionComponentCtrl = (function () {
            function AuctionComponentCtrl(dataAccessService, $interval) {
                var _this = this;
                this.dataAccessService = dataAccessService;
                this.$interval = $interval;
                this.areDetailsDisplayed = false;
                this.bidders = new Array(0);
                this.colors = ["#8049C9", "#93991C", "#221C99", "#991C1C"];
                this.$interval(function () {
                    var auctionResource = _this.dataAccessService.getSingleAuctionResource(_this.auction.Id);
                    auctionResource.get(function (data) {
                        _this.auction = new app.businessObjects.Auction().deserialize(data);
                    });
                }, 2000);
            }
            AuctionComponentCtrl.prototype.displayDetails = function () {
                this.areDetailsDisplayed = !this.areDetailsDisplayed;
            };
            AuctionComponentCtrl.prototype.didAuctionEnd = function () {
                return (Date.now() - this.auction.EndDate.getTime()) >= 0;
            };
            AuctionComponentCtrl.prototype.durationToEnd = function () {
                return (this.auction.EndDate.getTime() - Date.now()) / 1000 / 60;
            };
            AuctionComponentCtrl.prototype.chooseColor = function (username) {
                var index;
                if (this.didHandleBidder(username)) {
                    index = this.bidders.indexOf(username);
                }
                else {
                    index = this.bidders.length;
                    this.bidders.push(username);
                }
                index = index % this.colors.length;
                return this.colors[index];
            };
            AuctionComponentCtrl.prototype.didHandleBidder = function (username) {
                return (this.bidders.indexOf(username) != -1);
            };
            AuctionComponentCtrl.$inject = ["dataAccessService", "$interval"];
            return AuctionComponentCtrl;
        }());
        var AuctionComponentOptions = (function () {
            function AuctionComponentOptions() {
                this.bindings = {
                    // Here should come my binding's types.
                    auction: '<'
                };
                this.controller = AuctionComponentCtrl;
                this.controllerAs = "vm";
                this.templateUrl = "app/auctions/auctionComponent.html";
            }
            return AuctionComponentOptions;
        }());
        angular
            .module("auctionReporter")
            .component("auctionComponent", new AuctionComponentOptions());
    })(auctions = app.auctions || (app.auctions = {}));
})(app || (app = {}));
//# sourceMappingURL=auctionComponent.js.map