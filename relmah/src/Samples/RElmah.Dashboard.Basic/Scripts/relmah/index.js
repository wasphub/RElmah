(function($) {
    "use strict";

    function View(opts) {
        this.$ = $(opts.el);
        this.template = _.template($(opts.template).text());
    }

    View.prototype.render = function (data) {
        this.$.html(this.template({ data: data }));
    };

    function ErrorsView(opts) {
        View.apply(this, arguments);

        opts.errors.subscribe(this.render.bind(this));
    };

})($);