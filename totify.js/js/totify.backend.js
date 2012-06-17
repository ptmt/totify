   
$(document).ready(function () {
    
     
    var gridViewModel1 = function(items) {
        this.items = ko.observableArray(items);
     
        this.addReplace = function() {            
            if ($('.find').val()) {
                var new_item = { Find: $('.find').val(), Replace: $('.replace').val(), Actions: "delete" }; 
                console.log(new_item);    
                $.ajax({
                      url: "/panel/filter2.fs",
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
                { headerText: "Replace", rowText: "Replace" }              
               
            ],
            pageSize: 20
        });
    };

    var gridViewModel2 = function(items) {
        this.items = ko.observableArray(items);
     
        this.addQoute = function() {            
            if ($('textarea.qoute').val()) {
                var new_item = { Quote: $('.qoute').val(), Author: $('.author').val(), Actions: "delete" }; 
                console.log(new_item);    
                $.ajax({
                      url: "/panel/helper1.fs",
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
                { headerText: "Quote", rowText: "Quote" },
                { headerText: "Author", rowText: "Author" },               
                { headerText: "Actions", rowText: "Actions" } 
            ],
            pageSize: 20
        });
    };
     
    $.getJSON("/panel/filter2.fs", function(data) { 
        ko.applyBindings(new gridViewModel1(data), $('#filter2')[0]);
    })

    $.getJSON("/panel/helper1.fs", function(data) { 
        ko.applyBindings(new gridViewModel2(data), $('#helper1')[0]);
    })
    

});