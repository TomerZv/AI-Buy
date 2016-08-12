module app.auctions {
    interface IAuctionComponentBindings {
        // Here should come all the controller's bindings.
        auction: app.businessObjects.IAuction;
        areDetailsDisplayed: boolean;

        displayDetails(): void;
        didAuctionEnd(): boolean;
        durationToEnd(): number;
        chooseColor(username: string): Object;
    }

    interface IAuctionComponentCtrl extends IAuctionComponentBindings {
        // Here should come all the controller's functionality.
    }

    class AuctionComponentCtrl implements IAuctionComponentCtrl {
        public auction: app.businessObjects.IAuction;
        public areDetailsDisplayed: boolean;
        private colors: Array<string>;
        private bidders: Array<string>;

        static $inject = ["dataAccessService", "$interval"];
        constructor(
            private dataAccessService: app.common.DataAccessService,
            private $interval: ng.IIntervalService) {

            this.areDetailsDisplayed = false;
            this.bidders = new Array<string>(0);
            this.colors = ["#8049C9", "#93991C" , "#221C99", "#991C1C"];

            this.$interval(() => {
                var auctionResource = this.dataAccessService.getSingleAuctionResource(this.auction.Id);

                auctionResource.get((data: app.businessObjects.IAuction) => {
                    this.auction = new app.businessObjects.Auction().deserialize(data);
                });
            }, 2000);
        }

        displayDetails() : void {
            this.areDetailsDisplayed = !this.areDetailsDisplayed;
        }

        didAuctionEnd(): boolean {
            return (Date.now() - this.auction.EndDate.getTime()) >= 0;
        }

        durationToEnd(): number {
            return (this.auction.EndDate.getTime() - Date.now()) / 1000 / 60;
        }

        chooseColor(username: string): Object {
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
        }

        didHandleBidder(username: string): boolean {
            return (this.bidders.indexOf(username) != -1)
        }
    }

    class AuctionComponentOptions implements ng.IComponentOptions {

        public bindings: any;
        public controller: any;
        public controllerAs: string;
        public templateUrl: string;

        constructor() {
            this.bindings = {
                // Here should come my binding's types.
                auction : '<'
            };
            this.controller = AuctionComponentCtrl;
            this.controllerAs = "vm";
            this.templateUrl = "app/auctions/auctionComponent.html";
        }
    }

    angular
        .module("auctionReporter")
        .component("auctionComponent", new AuctionComponentOptions());
}  