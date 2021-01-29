SET GLOBAL event_scheduler = ON;

CREATE EVENT DailyReset ON SCHEDULE EVERY 1 DAY STARTS '2021-01-01 00:00:00' ON COMPLETION PRESERVE ENABLE DO
BEGIN
	DELETE FROM flower;
	
	DELETE 	FROM
		cq_item_owner_history 
	WHERE
		item_id IN (
		SELECT
			item.id 
		FROM
			cq_item item 
		WHERE
			player_id = 0 
			AND del_time IS NOT NULL 
		AND del_time < DATE_SUB( NOW(), INTERVAL 7 DAY ));
		
	DELETE 	FROM
		cq_item 
	WHERE
		player_id = 0 
		AND del_time IS NOT NULL 
	AND del_time < DATE_SUB( NOW(), INTERVAL 7 DAY );
	
	INSERT INTO daily_reset (run_time) VALUES (NOW());
END;