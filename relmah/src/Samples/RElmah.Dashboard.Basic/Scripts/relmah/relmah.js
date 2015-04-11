(function () {

    var root = window || this;

    var relmah = (function () {

        return function (endpoint, hub, subs) {
            var conn,
                errors,
                sources,
                recaps,
                apps = {},
                starting,
                valueOf = function () { return JSON.stringify(this); },
                getErrorTypes = function () {
                    return errors.groupBy(
                        function (e)    { return { SourceId: e.SourceId, Type: e.Error.Type, valueOf: valueOf }; },
                        function (e)    { return e; },
                        function (a, b) { return a.SourceId === b.SourceId && a.Type === b.Type; });
                };

            subs = subs || {};

            return {
                start: function (opts) {
                    conn         = $.hubConnection(endpoint);

                    var proxy    = conn.createHubProxy(hub || 'relmah-errors');

                    errors  = new Rx.Subject();
                    sources = new Rx.Subject();
                    recaps  = new Rx.Subject();

                    proxy.on('error', function (p) {
                        errors.onNext(p);
                    });

                    proxy.on('sources', function (existingApps, removedApps) {
                        existingApps.forEach(function (s) {
                            var k = s.SourceId;
                            !apps[k] && sources.onNext({ Source: s, Action: 'added' });
                            apps[k] = true;
                        });
                        removedApps && removedApps.forEach(function (s) {
                            var k = s.SourceId;
                            apps[k] && sources.onNext({ Source: s, Action: 'removed' });
                            apps[k] = false;
                        });
                    });

                    var groups = {};

                    proxy.on('recap', function (recap) {
                        groups['*'] && groups['*'].dispose();

                        var irs = recap.Sources
                            .map(function(a) {
                                return a.Types
                                    .map(function (b) { return { SourceId: a.SourceId, Type: b.Name, Measure: b.Measure }; });
                            });

                        [].concat.apply([], irs)
                            .forEach(function(ir) {
                                recaps.onNext({ SourceId: ir.SourceId, Type: ir.Type, Measure: ir.Measure });
                            });

                        groups['*'] = getErrorTypes()
                            .subscribe(function(et) {
                                var key = et.key.SourceId + '-' + et.key.Type;
                                groups[key] && groups[key].dispose();

                                var rs = recap.Sources
                                    .filter(function (a) { return a.SourceId === et.key.SourceId; })
                                    .map(function(a) {
                                        return a.Types
                                            .filter(function(b) { return b.Name === et.key.Type; })
                                            .map(function(b) { return b.Measure; });
                                    });
                        
                                var r = [].concat.apply([], rs)
                                    .reduce(function (a, c) { return a + c; }, 0);

                                groups[key] = et
                                    .scan(0, function (a) { return a + 1; })
                                    .subscribe(function (e) {
                                        recaps.onNext({ SourceId: et.key.SourceId, Type: et.key.Type, Measure: e + r });
                                    });
                            });
                    });

                    starting && starting();

                    conn.qs = { user: opts && opts.user };

                    return conn.start();
                },
                getErrors: function () { return errors; },
                getErrorTypes: getErrorTypes,
                getApplications: function () { return sources; },
                getRecaps: function () {
                    return recaps.groupBy(
                        function (e)    { return { SourceId: e.SourceId, Type: e.Type, valueOf: valueOf }; },
                        function (e)    { return e; },
                        function (a, b) { return a.SourceId === b.SourceId && a.Type === b.Type; });
                },
                stop: function () {
                    apps = {};
                    return conn && conn.stop();
                },
                starting: function(callback) {
                    starting = callback;
                },
                disconnected: function(callback) {
                    return conn.disconnected(callback);
                }
            };
        };
    })();

    root.relmah = relmah;

    if (typeof define == 'function' && typeof define.amd == 'object' && define.amd) {
        define(function() { return relmah; });
    }

}.call());