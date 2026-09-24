500 chunks:
| Max concurrent | Cumulative gen | Avg/chunk | Approx throughput |
| -------------: | -------------: | --------: | ----------------: |
|              1 |        357.4 s | 698.17 ms |     1.40 chunks/s |
|              2 |        208.6 s | 717.90 ms |     2.40 chunks/s |
|              3 |        133.5 s | 753.03 ms |     3.75 chunks/s |
|              4 |        118.2 s | 749.43 ms |     4.23 chunks/s |
|              5 |         97.6 s | 784.52 ms |     5.12 chunks/s |
|              6 |         83.3 s | 774.58 ms |     6.00 chunks/s |
|              7 |         75.2 s | 836.23 ms |     6.65 chunks/s |
|              8 |         65.8 s | 860.02 ms |     7.60 chunks/s |
|              9 |         64.9 s | 897.70 ms |     7.70 chunks/s |
|             10 |         61.8 s | 916.48 ms |     8.09 chunks/s |
|             12 |         33.6 s | 667.22 ms |    14.90 chunks/s |
|             14 |         32.4 s | 670.45 ms |    15.45 chunks/s |
|             16 |        43.8 s* | 622.05 ms |    11.42 chunks/s |
|             16 |         32.6 s | 670.58 ms |    15.33 chunks/s |
|             18 |         30.8 s | 630.51 ms |    16.23 chunks/s |
|             20 |         30.8 s | 630.58 ms |    16.23 chunks/s |
|             22 |         30.8 s | 631.93 ms |    16.23 chunks/s |
|             24 |         30.9 s | 627.88 ms |    16.19 chunks/s |
|             26 |         31.1 s | 635.35 ms |    16.10 chunks/s |
|             28 |         30.8 s | 623.88 ms |    16.24 chunks/s |
|             30 |         30.7 s | 627.17 ms |    16.30 chunks/s |
|             32 |         31.1 s | 631.61 ms |    16.09 chunks/s |
|             34 |         30.7 s | 626.27 ms |    16.27 chunks/s |
* The first 16-worker was anomalous; the rerun was 32.6s

Staged:
| Max concurrent | Biomes/chunk | Fill/chunk | Surface/chunk |
| -------------: | -----------: | ---------: | ------------: |
|              8 |        ~0 ms |  616.67 ms |       1.24 ms |
|             10 |        ~0 ms |  654.08 ms |       1.33 ms |
|             12 |        ~0 ms |  665.83 ms |       1.40 ms |
|             14 |        ~0 ms |  669.02 ms |       1.44 ms |
|             16 |        ~0 ms |  669.14 ms |       1.44 ms |
|             18 |        ~0 ms |  629.24 ms |       1.27 ms |
|             20 |        ~0 ms |  629.34 ms |       1.24 ms |
|             22 |        ~0 ms |  630.66 ms |       1.27 ms |
|             24 |        ~0 ms |  626.63 ms |       1.25 ms |
|             26 |        ~0 ms |  634.04 ms |       1.31 ms |
|             28 |        ~0 ms |  622.65 ms |       1.24 ms |
|             30 |        ~0 ms |  625.91 ms |       1.26 ms |
|             32 |        ~0 ms |  630.34 ms |       1.27 ms |
|             34 |        ~0 ms |  625.00 ms |       1.26 ms |

