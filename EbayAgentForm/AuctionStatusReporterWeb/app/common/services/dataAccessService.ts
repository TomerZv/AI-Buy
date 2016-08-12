module app.common {
    interface IDataAccessService {
        getAuctionResource(): ng.resource.IResourceClass<IAuctionResource>;
    }

    interface IAuctionResource
        extends ng.resource.IResource<app.businessObjects.IAuction> {

    }

    export class DataAccessService implements IDataAccessService {

        static $inject = ["$resource"];
        constructor(private $resource : ng.resource.IResourceService) {
        }

        getAuctionResource() : ng.resource.IResourceClass<IAuctionResource> {
            return this.$resource("http://localhost:8670/api/ManageAuctions/GetAuctions");
        }

        getSingleAuctionResource(id : number): ng.resource.IResourceClass<IAuctionResource> {
            return this.$resource("http://localhost:8670/api/ManageAuctions/GetAuction?id=" + id);
        }
    }

    angular
        .module("common.services")
        .service("dataAccessService", DataAccessService);
} 