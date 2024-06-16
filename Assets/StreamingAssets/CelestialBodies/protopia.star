{
  "id": "protopia",
  "name": "Protopia",
  "subtitle": "A star system in the making",
  "bodies": [
    {
      "type": "star",
      "id": "prol",
      "parent_id": null,
      "body_file": "core/prol.cel",
      "gal_coords_ly": {
        "x": 25550,
        "y": 57,
        "z": 0
      },
      "rotation": {
        "period_sec": 0,
        "obliquity_deg": 0,
        "rotation_at_epoch_deg": 0
      }
    },
    {
      "type": "planet",
      "id": "proterra",
      "parent_id": "prol",
      "body_file": "core/proterra.cel",
      "orbit": {
        "semimajor_axis_m": 1.49598023e+11,
        "eccentricity": 0.0167086,
        "inclination_deg": 0.0,
        "longitude_asc_node_deg": -11.26064,
        "arg_periapsis_deg": 1.9933,
        "mean_anomaly_epoch_deg": 358.617
      },
      "rotation": {
        "period_sec": 0,
        "obliquity_deg": 0,
        "rotation_at_epoch_deg": 0
      }
    },
    {
      "type": "planet",
      "id": "proluna",
      "parent_id": "proterra",
      "body_file": "core/proluna.cel",
      "orbit": {
        "semimajor_axis_m": 384399000,
        "eccentricity": 0.0549,
        "inclination_deg": 5.145,
        "longitude_asc_node_deg": 125.08,
        "arg_periapsis_deg": 318.15,
        "mean_anomaly_epoch_deg": 135.27
      },
      "rotation": {
        "period_sec": 0,
        "obliquity_deg": 0,
        "rotation_at_epoch_deg": 0
      }
    }
  ]
}