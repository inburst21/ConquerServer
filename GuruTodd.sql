
DELETE FROM cq_npc WHERE id=13 LIMIT 1;
DELETE FROM cq_action WHERE id>=15000 AND id<16000 LIMIT 1000;
DELETE FROM cq_task WHERE id>=15000 AND id<16000 LIMIT 1000;

INSERT INTO `cq_npc` VALUES ('0013', '0', '0', 'GuruTodd', '0002', '7310', '-1', '1002', '0441', '0382', '15000', '0000', '0000', '0000', '0000', '0000', '0000', '0000', '0', '0', '0', '0', '', '0000', '00', '00', '0000', '00', '0000', '0', '0');

INSERT INTO `cq_action` VALUES ('15000', '15003', '15001', '0123', '0', '2016-09-19 18:00 2025-09-26 23:59');
INSERT INTO `cq_action` VALUES ('15001', '15002', '0000', '0101', '0', 'Sorry! The event has ended, please come back earlier next time.');
INSERT INTO `cq_action` VALUES ('15002', '610027', '0000', '0102', '0', 'Damn~it! 0');
INSERT INTO `cq_action` VALUES ('15003', '15004', '0000', '0101', '0', 'Hello! I can give you 5x experience multiplier for 1 hour until september, 26. Do you want it?');
INSERT INTO `cq_action` VALUES ('15004', '15005', '0000', '0102', '0', 'I~want~to~claim~it. 15010');
INSERT INTO `cq_action` VALUES ('15005', '610027', '0000', '0102', '0', 'I~changed~my~mind. 0');

INSERT INTO `cq_action` VALUES ('15010', '15012', '15001', '0123', '0', '2020-05-16 18:00 2025-09-26 23:59');
INSERT INTO `cq_action` VALUES ('15012', '15013', '0000', '1010', '2005', 'From now on, you can get double experience for the next hour.');
INSERT INTO `cq_action` VALUES ('15013', '0000', '0000', '1048', '0', '200 3600');

INSERT INTO `cq_task` VALUES ('15000', '15000', '0000', '', '', '0', '0', '999', '-100000', '100000', '0999', '0000', '0', '-1', '0');
INSERT INTO `cq_task` VALUES ('15010', '15010', '0000', '', '', '0', '0', '999', '-100000', '100000', '0999', '0000', '0', '-1', '0');
