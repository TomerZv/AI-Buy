var app;
(function (app) {
    var businessObjects;
    (function (businessObjects) {
        (function (AuctionStatus) {
            AuctionStatus[AuctionStatus["Future"] = 0] = "Future";
            AuctionStatus[AuctionStatus["Open"] = 1] = "Open";
            AuctionStatus[AuctionStatus["Close"] = 2] = "Close";
        })(businessObjects.AuctionStatus || (businessObjects.AuctionStatus = {}));
        var AuctionStatus = businessObjects.AuctionStatus;
        var Auction = (function () {
            function Auction() {
            }
            Auction.prototype.getStatus = function () {
                // TEMP - change this.
                return AuctionStatus.Close;
            };
            Auction.prototype.getCurrentBid = function () {
                var bid;
                if (this.Biddings.length == 0) {
                    bid = null;
                }
                else {
                    bid = this.Biddings[this.Biddings.length - 1];
                }
                return bid;
            };
            Auction.prototype.getCurrentPrice = function () {
                return this.getCurrentBid().Price;
            };
            Auction.prototype.deserialize = function (input) {
                var _this = this;
                this.Id = input.Id;
                this.ItemType = input.ItemType;
                this.StartDate = new Date(input.StartDate);
                this.Duration = input.Duration;
                this.EndDate = new Date(input.EndDate);
                this.MinimumPrice = input.MinimumPrice;
                this.AvgPrice = input.AvgPrice;
                this.MinBid = input.MinBid;
                this.Biddings = new Array(0);
                input.Biddings.forEach(function (bid) { return _this.Biddings.push(new businessObjects.Bid().deserialize(bid)); });
                return this;
            };
            return Auction;
        }());
        businessObjects.Auction = Auction;
    })(businessObjects = app.businessObjects || (app.businessObjects = {}));
})(app || (app = {}));
//# sourceMappingURL=auction.js.map