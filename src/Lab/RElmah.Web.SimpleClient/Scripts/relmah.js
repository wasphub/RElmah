relmah = (function() {
    "use strict";

    return function (endpoint) {
        var errors        = new Rx.Subject(),
            clusters      = new Rx.Subject(),
            applications  = new Rx.Subject();

        var errs;

        var getErrors     = function(ext) {
            errs = errs || (typeof (ext) === 'function' && ext(errors) || errors);
            return errs;
        };

        return {
            start: function() {
                var conn  = $.hubConnection(endpoint),
                    proxy = conn.createHubProxy('relmah');

                proxy.on('error', function (p) {
                    errors.onNext(p);
                });
                proxy.on('clusterOperation', function (c) {
                    clusters.onNext(c);
                });
                proxy.on('applicationOperation', function (c) {
                    applications.onNext(c);
                });

                return conn.start().done(function() {
                    proxy.invoke('getErrors').done(function (es) {
                        for (var i = 0; i < es.length; i++) {
                            errors.onNext(es[i]);
                        }
                    });
                });
            },
            getErrors: getErrors,
            getClusters: function (ext) {
                return typeof (ext) === 'function' && ext(clusters) || clusters;
            },
            getApplications: function (ext) {
                return typeof (ext) === 'function' && ext(applications) || applications;
            }
        };
    };
})();