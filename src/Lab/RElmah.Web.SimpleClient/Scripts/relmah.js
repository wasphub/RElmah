relmah = (function() {
    "use strict";

    return function (endpoint) {
        var errors = new Rx.Subject(),
            clusters = new Rx.Subject();

        return {
            start: function() {
                var conn   = $.hubConnection(endpoint),
                    relmah = conn.createHubProxy('relmah');

                relmah.on('error', function (p) {
                    errors.onNext(p);
                });
                relmah.on('clusterUpdate', function (c) {
                    clusters.onNext(c);
                });

                return conn.start();
            },
            errors: errors,
            clusters: clusters
        };
    };
})();