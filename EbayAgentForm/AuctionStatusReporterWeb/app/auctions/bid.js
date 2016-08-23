var app;
(function (app) {
    var businessObjects;
    (function (businessObjects) {
        var Bid = (function () {
            function Bid() {
            }
            Bid.prototype.deserialize = function (input) {
                this.Id = input.Id;
                this.AuctionId = input.AuctionId;
                this.Price = input.Price;
                this.Username = input.Username;
                this.Date = new Date(input.Date);
                return this;
            };
            return Bid;
        }());
        businessObjects.Bid = Bid;
    })(businessObjects = app.businessObjects || (app.businessObjects = {}));
})(app || (app = {}));
//# sourceMappingURL=bid.js.map