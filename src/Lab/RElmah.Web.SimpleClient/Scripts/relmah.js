relmah = (function() {
    "use strict";

    return function (endpoint) {
        var errors = new Rx.Subject();

        return {
            start: function() {
                var conn = $.hubConnection(endpoint),
                    relmah = conn.createHubProxy('relmah');

                relmah.on('dispatch', function (p) {
                    errors.onNext(p);
                });

                return conn.start();
            },
            errors: errors
        };
    };
})();