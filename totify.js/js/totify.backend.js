   
$(document).ready(function () {
    
     
    var PagedGridModel = function(items) {
        this.items = ko.observableArray(items);
     
        this.addItem = function() {            
            if ($('#newItem .find').val() && $('#newItem .replace').val()) {
                var new_item = { Find: $('#newItem .find').val(), Replace: $('#newItem .replace').val(), Actions: "delete" }; 
                console.log(new_item);    
                $.ajax({
                      url: "/filter2/backend.fs",
                      data: new_item,
                      type: 'PUT'
                }).done(function(e) { 
                    console.log(e);                
                });                   
                this.items.push(new_item);
            }
        };

        this.removeItem = function(item) {

        }
     
        this.sortByName = function() {
            this.items.sort(function(a, b) {
                return a.name < b.name ? -1 : 1;
            });
        };
     
        this.jumpToFirstPage = function() {
            this.gridViewModel.currentPageIndex(0);
        };
     
        this.gridViewModel = new ko.simpleGrid.viewModel({
            data: this.items,
            columns: [
                { headerText: "Find", rowText: "Find" },
                { headerText: "Replace", rowText: "Replace" },               
                { headerText: "Actions", rowText: "Actions" } 
            ],
            pageSize: 20
        });
    };
     
    $.getJSON("/filter2/backend.fs", function(data) { 
        ko.applyBindings(new PagedGridModel(data));
    })
    

});