relmah = (function() {
    "use strict";

    return function (endpoint, subs) {
        var errors        = new Rx.Subject(),
            clusters      = new Rx.Subject(),
            applications  = new Rx.Subject(),
            es, cs, as;

        subs = subs || {};

        return {
            start: function() {
                var conn = $.hubConnection(endpoint),
                    proxy = conn.createHubProxy('relmah');

                proxy.on('error', function(p) {
                    errors.onNext(p);
                });
                proxy.on('clusterOperation', function(c) {
                    clusters.onNext(c);
                });
                proxy.on('applicationOperation', function(c) {
                    applications.onNext(c);
                });

                return conn.start().done(function() {
                    proxy.invoke('getErrors').done(function(errs) {
                        var targets = errs.Targets;
                        for (var i = 0; i < targets.length; i++) {
                            errors.onNext(targets[i]);
                        }
                    });
                });
            },
            getErrors: function() {
                es = es || (subs.errors && typeof (subs.errors) === 'function' && subs.errors(errors) || errors);
                return es;
            },
            getClusters: function() {
                cs = cs || (subs.clusters && typeof (subs.clusters) === 'function' && subs.clusters(clusters) || clusters);
                return cs;
            },
            getApplications: function() {
                as = as || (subs.applications && typeof (subs.applications) === 'function' && subs.applications(applications) || applications);
                return as;
            }
        };
    };
})();